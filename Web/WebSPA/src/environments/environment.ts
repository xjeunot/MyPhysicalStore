// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environmentServer = {
  serverStoreApi: "dev.docker.local:44383",
  serverCustomerApi: "dev.docker.local:44384",
  serverIdentity : "dev.docker.local:44385"
}

export const environment = {

  production: false,

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

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
