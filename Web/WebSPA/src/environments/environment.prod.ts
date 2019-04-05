export const environmentServer = {
  serverStoreApi: "",
  serverCustomerApi: "",
  serverIdentity : ""
}

export const environment = {

  production: true,

  api : {
    cashDesk: {
      getAll :  "https://" + environmentServer.serverStoreApi + "/api/v1/CashDesk",
      getId :   "https://" + environmentServer.serverStoreApi + "/api/v1/CashDesk/{{ID}}",
      post :    "https://" + environmentServer.serverStoreApi + "/api/v1/CashDesk",
      put :     "https://" + environmentServer.serverStoreApi + "/api/v1/CashDesk",
      remove :  "https://" + environmentServer.serverStoreApi + "/api/v1/CashDesk/{{ID}}"
    },
    checkOut: {
      getAll :  "https://" + environmentServer.serverStoreApi + "/api/v1/CheckOut",
      getId :   "https://" + environmentServer.serverStoreApi + "/api/v1/CheckOut/{{ID}}",
      post :    "https://" + environmentServer.serverStoreApi + "/api/v1/CheckOut",
      put :     "https://" + environmentServer.serverStoreApi + "/api/v1/CheckOut",
      remove :  "https://" + environmentServer.serverStoreApi + "/api/v1/CheckOut/{{ID}}"
    },
    customer: {
      getAll :  "https://" + environmentServer.serverCustomerApi + "/api/v1/Customer",
      getId :   "https://" + environmentServer.serverCustomerApi + "/api/v1/Customer/{{ID}}",
      post :    "https://" + environmentServer.serverCustomerApi + "/api/v1/Customer",
      put :     "https://" + environmentServer.serverCustomerApi + "/api/v1/Customer",
      remove :  "https://" + environmentServer.serverCustomerApi + "/api/v1/Customer/{{ID}}"
    }
  },
  identity : {
    token : 'https://' + environmentServer.serverIdentity + '/connect/token',
    revoke : 'https://' + environmentServer.serverIdentity + '/connect/revoke'
  }
};
