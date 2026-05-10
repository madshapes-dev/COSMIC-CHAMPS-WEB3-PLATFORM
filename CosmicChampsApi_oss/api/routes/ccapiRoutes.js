'use strict';
module.exports = function(app) {
  var walletList = require('../controllers/ccapiController');
  var matchList = require('../controllers/matchController');
  var inventoryList = require('../controllers/inventoryController');

//   /*
//   app.route('/wallets')
//     .get(walletList.list_all_wallet)
//     .post(walletList.create_a_wallet);

//   app.route('/wallets/:walletId')
//     .get(walletList.read_a_wallet)
//     .put(walletList.update_a_wallet)
//     .delete(walletList.delete_a_wallet);
// */

 // app.route('/addwallet').post(walletList.create_new_wallet);

  app.route('/check/:walletId').get(walletList.checkaddress);
  app.route('/updatewallets').get(walletList.updatedb);


 

  app.route('/matchstart/:matchId/:pa/:pb').get(matchList.matchstart);
  app.route('/matchend/:matchId/:w').get(matchList.matchend);

  app.route('/getMatchdata/:from').get(matchList.getMatchdata);


  //nope
  //app.route('/list_all').get(matchList.list_all);
  
  //only updates uncommon
  //app.route('/updateinventory').get(inventoryList.updatedb);


  app.route('/getinventory/:walletId').get(inventoryList.getInventory);


  app.route('/updatespecials_all').get(inventoryList.updatedspecials);


};
