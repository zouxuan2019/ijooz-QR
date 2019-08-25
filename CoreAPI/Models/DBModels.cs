using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static CoreAPI.Models.ProductList;
using static CoreAPI.Models.QrBalanceModel;
using static CoreAPI.Models.UseHistory;

namespace CoreAPI.Models
{
    public class DBModels
    {
        public async Task<string> checkTransId(string UserID, string TransId)
        {
            
            if (TransId.Length > 50)
            {
                return "TransId must less than 50";
            }

            string result = "";
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"SELECT [UserID],[TransId]  FROM [membershipQR].[dbo].[QR_Transaction] where UserID=@UserID and TransId=@TransId order by 1,2";

                if ((await conn.QueryAsync(strSql, new { UserID = UserID, TransId = TransId })).Any())
                    result= "Duplicated TransId";
            }

            return result;
        }

        public async Task<List<ProductDetail>> GetAllProductDetail()
        {
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"SELECT [Company]
      ,[Product]
      ,[Currency]
      ,[UnitPrice]
  FROM [MembershipQR].[dbo].[ProductList] where ifvalid=1 order by 1,2";

                return (await conn.QueryAsync<ProductDetail>(strSql)).ToList();//, new { ids = reqForm.UserID }

            }
        }

        public async Task<List<ProductDetail>> GetAllProductDetail(string Company)
        {
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"SELECT [Company]
      ,[Product]
      ,[Currency]
      ,[UnitPrice]
  FROM [MembershipQR].[dbo].[ProductList] where ifvalid=1 and Company=@Company order by 1,2";

                return (await conn.QueryAsync<ProductDetail>(strSql, new { Company = Company })).ToList();

            }

        }

        public async Task<List<ProductDetail>> GetProductDetailFromProductList(string Company, string Product)
        {
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"SELECT [Company]
      ,[Product]
      ,[Currency]
      ,[UnitPrice]
  FROM [MembershipQR].[dbo].[ProductList] where ifvalid=1 and Company=@Company and Product=@Product";

                return (await conn.QueryAsync<ProductDetail>(strSql, new { Company = Company, Product = Product })).ToList();

            }
        }

        public async Task<List<QrBalance>> GetQRBalance(string UserID)
        {
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"SELECT [UserID]
      ,[QRCode]
      ,[Company]      
      ,[Product]
      ,[Currency]
      ,[UnitPrice]
      ,[ExpiryDate], [Qty]
      ,[Remark]
      ,[LastUpdateTime]
  FROM [MembershipQR].[dbo].[QR_Status] where ifValid=1 and [UserID]=@UserID order by [ExpiryDate]";

                return (await conn.QueryAsync<QrBalance>(strSql, new { UserID = UserID })).ToList();
            }
        }

        public async Task<List<UseDetail>> GetQRUseDetail(string UserID, string lastXMonth)
        {
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"SELECT [QRCode]
      ,[MachineID]
      ,[TransID]
      ,[TransTime]
      ,[Currency]
      ,[UnitPrice]
  FROM [dbo].[QR_Use_History] where [UserID] = @ids and [TransTime]>dateadd(month, -@lastXMonth, getdate()) order by TransTime desc";

                return (await conn.QueryAsync<UseDetail>(strSql, new { ids = UserID, lastXMonth= Convert.ToInt16(lastXMonth) })).ToList();
            }
        }


        public async Task<List<QrBalance>> GetProductDetailFromQR_Status(string UserID, string QRCode)
        {
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"SELECT [UserID]
      ,[QRCode]
      ,[Company]
      ,[Qty]
      ,[Product]
      ,[Currency]
      ,[UnitPrice]
      ,[ExpiryDate], Remark
      ,[LastUpdateTime]
  FROM [MembershipQR].[dbo].[QR_Status] where ifvalid=1 and QRCode=@QRCode and UserID=@UserID";

                return (await conn.QueryAsync<QrBalance>(strSql, new { QRCode = QRCode, UserID = UserID })).ToList();

            }
        }

        public async Task InitNewQRCode(string UserID, string TransId, DateTime TransTime, int Qty, double Amt, string PromotionCode)
        {
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"insert into [MembershipQR].[dbo].[QR_Transaction]([UserID]
      ,[TransId]
      ,[TransTime]
      ,[ActionType]
      ,[Qty]
      ,[SourceQR]
      ,[Amt]
      ,[PromotionCode]) values (@UserID, @TransId, @TransTime, 'NewQR', @Qty, '', @Amt, @PromotionCode)";

                 await conn.ExecuteAsync(strSql, new { UserID = UserID, TransId = TransId, TransTime= TransTime, Qty= Qty, Amt= Amt, PromotionCode= PromotionCode });

            }
        }
        public async Task SaveNewQRCode(string UserID, string TransId, string QRCode, string Company, int Qty, string Product, string Currency, double UnitPrice, DateTime ExpiryDate)
        {
            string strSql = @"update [MembershipQR].[dbo].[QR_Transaction] set [QRCode]=@QRCode, ifSuccess=1
      where UserID = @UserID and TransId = @TransId and QRCode is null";
            string strSql2 = @"insert into [MembershipQR].[dbo].[QR_Status] ([UserID]
      ,[QRCode]
      ,[Company]
      ,[Qty]
      ,[Product]
      ,[Currency]
      ,[UnitPrice]
      ,[ExpiryDate]
      ,[ifValid]
      ,[LastUpdateTime]) values(@UserID, @QRCode, @Company, @Qty, @Product, @Currency, @UnitPrice, @ExpiryDate, 1, getdate())";

            using (var tranScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
                {
                    await conn.ExecuteAsync(strSql, new { QRCode = QRCode, UserID = UserID, TransId = TransId });
                    await conn.ExecuteAsync(strSql2, new
                    {
                        UserID = UserID,
                        QRCode = QRCode,
                        Company = Company,
                        Qty = Qty,
                        Product = Product,
                        Currency = Currency,
                        UnitPrice = UnitPrice,
                        ExpiryDate = ExpiryDate
                    });
                    await conn.OpenAsync();

                }
                tranScope.Complete();

            }


        }

        public async Task InitTopUpQRCode(string UserID, string QRCode, string TransId, DateTime TransTime, int TopUpQty, double Amt, string PromotionCode)
        {
            //only update QRCode when transaction is done successfully
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"insert into [MembershipQR].[dbo].[QR_Transaction]([UserID]
      ,[TransId]
      ,[TransTime]
      ,[ActionType]
      ,[Qty]
      ,[QRCode]
      ,[Amt]
      ,[PromotionCode]) values (@UserID, @TransId, @TransTime, 'TopUpQR', @Qty, @QRCode, @Amt, @PromotionCode)";

                await conn.ExecuteAsync(strSql, new { UserID = UserID, TransId = TransId, TransTime = TransTime, Qty = TopUpQty, QRCode= QRCode, Amt = Amt, PromotionCode = PromotionCode });

            }
        }
        public async Task SaveTopUpQRCode(string UserID, string TransId, string QRCode, int TtlQty, DateTime ExpiryDate)
        {
            string strSql = @"update [MembershipQR].[dbo].[QR_Transaction] set ifSuccess=1 where UserID = @UserID and TransId = @TransId and QRCode =@QRCode";
            string strSql2 = @"update [MembershipQR].[dbo].[QR_Status] set [Qty]=@Qty, ExpiryDate = @ExpiryDate where UserID=@UserID and QRCode=@QRCode";

            using (var tranScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
                {
                    await conn.ExecuteAsync(strSql, new { UserID = UserID, TransId = TransId, QRCode= QRCode });
                    await conn.ExecuteAsync(strSql2, new
                    {
                        UserID = UserID,
                        QRCode = QRCode, 
                        Qty = TtlQty,
                        ExpiryDate = ExpiryDate
                    });
                    await conn.OpenAsync();

                }
                tranScope.Complete();

            }


        }

        public async Task InitShareQRCode(string UserID, string QRCode, string TransId, DateTime TransTime, int SharedQty)
        {
            //only update QRCode when transaction is done successfully
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"insert into [MembershipQR].[dbo].[QR_Transaction]([UserID]
      ,[TransId]
      ,[TransTime]
      ,[ActionType]
      ,[Qty]
      ,[SourceQR]) values (@UserID, @TransId, @TransTime, 'ShareQR', -@SharedQty, @QRCode)";

                await conn.ExecuteAsync(strSql, new { UserID = UserID, TransId = TransId, TransTime = TransTime, SharedQty = SharedQty, QRCode = QRCode});

            }
        }
        public async Task SaveShareQRCode(string UserID, string TransId, string QRCode, int RemainQty, string SharedQRCode, int SharedQty, string Remark)
        {
            string strSql = @"update [MembershipQR].[dbo].[QR_Transaction] set ifSuccess=1, QRCode=@SharedQRCode where UserID = @UserID and TransId = @TransId and SourceQR =@QRCode";
            string strSql2 = @"insert into[MembershipQR].[dbo].[QR_Status]
        ([UserID],[QRCode],[Qty],[Company],[Product],[Currency],[UnitPrice],[ExpiryDate],[ifValid],[LastUpdateTime], Remark) 
select @SharedUserID [UserID],@SharedQRCode [QRCode],@SharedQty [Qty],
[Company],[Product],[Currency],[UnitPrice],[ExpiryDate],[ifValid],getdate() [LastUpdateTime], @Remark Remark 
from [MembershipQR].[dbo].[QR_Status] 
where UserID=@UserID and QRCode=@QRCode";
            string strSql3 = @"update [MembershipQR].[dbo].[QR_Status] set [Qty]=@Qty where UserID=@UserID and QRCode=@QRCode";

            using (var tranScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
                {
                    await conn.ExecuteAsync(strSql, new { UserID = UserID, TransId = TransId, QRCode = QRCode, SharedQRCode= SharedQRCode });
                    await conn.ExecuteAsync(strSql2, new
                    {
                        SharedUserID = "Shared_" + UserID,
                        SharedQRCode = SharedQRCode,
                        SharedQty = SharedQty,          
                        Remark=Remark,
                        UserID=UserID,
                        QRCode= QRCode
                    });
                    await conn.ExecuteAsync(strSql3, new
                    {
                        UserID = UserID,
                        QRCode = QRCode,
                        Qty = RemainQty
                    });
                    await conn.OpenAsync();

                }
                tranScope.Complete();

            }


        }

        public async Task InitRegenQRCode(string UserID, string QRCode, string TransId, DateTime TransTime)
        {
            //only update QRCode when transaction is done successfully
            using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
            {
                string strSql = @"insert into [MembershipQR].[dbo].[QR_Transaction]([UserID]
      ,[TransId]
      ,[TransTime]
      ,[ActionType]
      ,[Qty]
      ,[SourceQR]) select [UserID], @TransId TransId, @TransTime TransTime, 'RegenQR', -[Qty], [QRCode] from [MembershipQR].[dbo].[QR_Status] 
where UserID=@UserID and QRCode=@QRCode";

                await conn.ExecuteAsync(strSql, new { UserID = UserID, TransId = TransId, TransTime = TransTime, QRCode = QRCode });

            }
        }
        public async Task SaveRegenQRCode(string UserID, string TransId, string QRCode, string NewQRCode, int balanceQty)
        {
            //update transaction
            string strSql = @"update [MembershipQR].[dbo].[QR_Transaction] set ifSuccess=1, QRCode=@NewQRCode where UserID = @UserID and TransId = @TransId and SourceQR =@QRCode";
            //Insert new QR
            string strSql2 = @"insert into[MembershipQR].[dbo].[QR_Status]
        ([UserID],[QRCode],[Qty],[Company],[Product],[Currency],[UnitPrice],[ExpiryDate],[ifValid],[LastUpdateTime]) 
select [UserID],@NewQRCode [QRCode], @balanceQty [Qty], [Company],[Product],[Currency],[UnitPrice],[ExpiryDate],[ifValid],getdate() [LastUpdateTime] 
from [MembershipQR].[dbo].[QR_Status] 
where UserID=@UserID and QRCode=@QRCode";
            //disable old QR
            string strSql3 = @"update [MembershipQR].[dbo].[QR_Status] set ifValid=0 where UserID=@UserID and QRCode=@QRCode";

            using (var tranScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
                {
                    await conn.ExecuteAsync(strSql, new { UserID = UserID, TransId = TransId, QRCode = QRCode, NewQRCode = NewQRCode });
                    await conn.ExecuteAsync(strSql2, new
                    {
                        NewQRCode = NewQRCode,
                        UserID = UserID,
                        QRCode = QRCode,
                        balanceQty = balanceQty
                    });
                    await conn.ExecuteAsync(strSql3, new
                    {
                        UserID = UserID,
                        QRCode = QRCode
                    });
                    await conn.OpenAsync();

                }
                tranScope.Complete();

            }


        }

        public async Task SaveUseHistory(string QRCode, string MachineID, string TransId, DateTime TransTime, int balanceQty, DateTime ExpiryDate)
        {
            //Insert use history
            string strSql = @"insert into [MembershipQR].[dbo].[QR_Use_History]([QRCode]
      ,[MachineID]
      ,[TransID]
      ,[TransTime]) values (@QRCode, @MachineID, @TransId, @TransTime)";
            
            string strSql2 = @"update [MembershipQR].[dbo].[QR_Status] set [Qty]=@balanceQty, [ExpiryDate]=@ExpiryDate where QRCode=@QRCode";
            
            string strSql3 = @"update a set [UserID]=b.[UserID], [Currency]=b.Currency,[UnitPrice]=b.UnitPrice from [MembershipQR].[dbo].[QR_Use_History] a, [dbo].[QR_Status] b where a.UserID is null and a.QRCode=b.QRCode";

            using (var tranScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (SqlConnection conn = new SqlConnection(Startup.ConnectionString))
                {
                    await conn.ExecuteAsync(strSql, new { MachineID = MachineID, TransId = TransId, QRCode = QRCode, TransTime = TransTime });
                    await conn.ExecuteAsync(strSql2, new
                    {
                        balanceQty = balanceQty,
                        ExpiryDate = ExpiryDate,
                        QRCode = QRCode
                    });
                    await conn.ExecuteAsync(strSql3);
                    await conn.OpenAsync();

                }
                tranScope.Complete();
            }

        }


    }
}
