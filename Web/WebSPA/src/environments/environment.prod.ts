export const environmentServer = {
  serverStoreApi: "/api/store",
  serverCustomerApi: "/api/customer",
  serverIdentity : "/api/identity"
}

export const environment = {

  production: true,

  api : {
    storeApi : {
      cashDesk: {
        getAll :  environmentServer.serverStoreApi + "/v1/CashDesk",
        getId :   environmentServer.serverStoreApi + "/v1/CashDesk/{{ID}}",
        post :    environmentServer.serverStoreApi + "/v1/CashDesk",
        put :     environmentServer.serverStoreApi + "/v1/CashDesk",
        remove :  environmentServer.serverStoreApi + "/v1/CashDesk/{{ID}}"
      },
      checkOut: {
        getAll :  environmentServer.serverStoreApi + "/v1/CheckOut",
        getId :   environmentServer.serverStoreApi + "/v1/CheckOut/{{ID}}",
        post :    environmentServer.serverStoreApi + "/v1/CheckOut",
        put :     environmentServer.serverStoreApi + "/v1/CheckOut",
        remove :  environmentServer.serverStoreApi + "/v1/CheckOut/{{ID}}"
      }
    },
    customerApi : {
      customer: {
        getAll :  environmentServer.serverCustomerApi + "/v1/Customer",
        getId :   environmentServer.serverCustomerApi + "/v1/Customer/{{ID}}",
        post :    environmentServer.serverCustomerApi + "/v1/Customer",
        put :     environmentServer.serverCustomerApi + "/v1/Customer",
        remove :  environmentServer.serverCustomerApi + "/v1/Customer/{{ID}}"
      }
    }
  },
  identity : {
    token : environmentServer.serverIdentity + '/connect/token',
    revoke : environmentServer.serverIdentity + '/connect/revoke'
  }
};
