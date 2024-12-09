using Blazored.LocalStorage;

namespace UnphuCard_RecargaFront.Data
{
    

        public class LocalStorage
        {
            private readonly ILocalStorageService _localStorage;

            public LocalStorage(ILocalStorageService localStorage)
            {
                _localStorage = localStorage;
            }

            public async Task<int> GetPaymentMethodAsync()
            {
                var storedPaymentMethod = await _localStorage.GetItemAsync<int>("paymentMethod");
                return storedPaymentMethod;
            }

            public async Task<decimal> GetAmountAsync()
            {
                var storedAmount = await _localStorage.GetItemAsync<decimal>("amount");
                return storedAmount;
            }
        }

    
}
