namespace Application.Constants
{
    public class OrderConst
    {
        public class OrderSystemStatus
        {
            public const int Canceled = 99;
            public const int Finished = 88;

            public const int Process = 110;
            public const int InProcess = 210;

            public const int Stock = 210;
            public const int PrepareRelease = 310;
        }

        public class OrderStatus
        {
            public const int WaitingWarehouseConfirm = 11;
            public const int WaitingImportProduct = 22;
            public const int ImportingProduct = 33;
            public const int FinishImportProduct = 44;
            public const int PendingSend = 55;
            public const int Delivering = 66;
            public const int Finished = 77;
        }

        public class InProcessStatus
        {
            public const int PendingConfirmImportProduct = 207;
            public const int ConfirmImportProduct = 208;
            public const int ImportingProduct = 209;
            public const int WaitCustomerApprove = 211;
            public const int CustomerApproved = 212;
            public const int Return = 213;
            public const int Renew = 214;

            public const int PendingSend = 215;
            public const int ReceivingToVietNam = 216;
            public const int ReceivingToCustomer = 217;
            public const int CustomerHasReceived = 218;
        }

        public class SourceFrom
        {
            public const string CustomerOrder = "CUSTOMER_ORDER";
            public const string SaleCreate = "SALE_CREATE_ORDER";
        }
    }
}