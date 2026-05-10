'use strict';



const NODEURL = "https://mainnet-idx.algonode.cloud";


const request = require('sync-request');

var mongoose = require('mongoose'),
Inventory = mongoose.model('Inventories'),
NftSnapshot = mongoose.model('NftSnapshots');



const NFTS = require('../constants/gen1nftlist.js');
const BOOSTER = require('../constants/boostvalues.js');



//SPECIALS
//red chroma, 1k mints, space rocks
//cron function
//
exports.updatedbcron_ps = function(req, res) {

  /// console.log(NFTS.UNCOMMON);
 
   //iterat over all models for this rarity
   var selectedrarity = "SPECIAL";
 
   NftSnapshot.find({rarity:selectedrarity}, function(err, nftlist) {
     if (err){
       //res.send(err);
     }else{
 
       for (const [model, skins] of Object.entries(NFTS[selectedrarity])) {
         //console.log(`${model}: ${skins}`);
         //iterate through all skins for this model (e.g. all cybi skins)
         for (var j = 0; j < skins.length; j++){
           //array of objects (skins)
           //[{ name: 'Cybi 2', id: 786130582 },...]
           var resp = request('GET', `${NODEURL}/v2/assets/${skins[j].id}/balances?limit=1000&currency-greater-than=0`, {
             headers: {
               'user-agent': 'example-user-agent',
             },
           });
 
           var bodydata = JSON.parse(resp.getBody('utf8'));
           //console.log(bodydata);

           var holderstemp = [];
           
           for(let wal = 0; wal < bodydata.balances.length ;wal++){
 
             var current = bodydata.balances[wal];
             if(!current.deleted){
               //console.log(current.address);
               holderstemp.push({ _id: current.address, count:current.amount});
               //holderstemp[current.address] = current.amount;
             }
           }
 
           
 
           var alreadypresent = false;
           for(let s = 0; s < nftlist.length ;s++){
             //console.log(nftlist[s]._id + "---"+ skins[j].id); 
             if(nftlist[s]._id == skins[j].id){
               //exisiting one
               alreadypresent = true;
               break;
             }
           }
 
 
           if(alreadypresent){
             //update only holders field
             var update = 
             {
                 holders:holderstemp,
                 updated_date:Date.now()
             };
             var filter = {_id: skins[j].id};
         
             NftSnapshot.findOneAndUpdate(filter, update, {new: true}, function(errx, msgx) {
                 if (errx){
                   //res.send(errx);
                   //console.log(errx);
                 }else{
                   //res.json(nftsnap);
                   //console.log("updatedto->"+msgx);
                 }
               });
 
           }else{
             //enter new document
             var skinsnap = new NftSnapshot({
               _id:skins[j].id,
               rarity:selectedrarity,
               base:model,
               name:skins[j].name,
               holders:holderstemp,
             });
 
             //console.log(skinsnap);;
 
             skinsnap.save(function(errx, msgx) {
               if (errx){
                 //res.send(err);
                 console.log("err"+errx);
                 //will throw err on duplicate key... nothign of importance tbh
               }
               else{
                 //res.json(matchh);
                 //console.log("inserted->"+msgx);
               } 
             });
           }
 
             //console.log(holderstemp);
 
         }//end iterate skins for this champ
 
       }//end iterate champs of this rarity
 
     }
 
   });
 
 };


//manual call for specials
exports.updatedspecials = function(req, res) {

  /// console.log(NFTS.UNCOMMON);
 
   //iterat over all models for this rarity
   var selectedrarity = "SPECIAL";
 
   NftSnapshot.find({rarity:selectedrarity}, function(err, nftlist) {
     if (err){
       res.send(err);
     }else{
 
       for (const [model, skins] of Object.entries(NFTS[selectedrarity])) {
         //cybi: [object Object]
         //tertius: [object Object]
         //coyote: [object Object]
         //hammer: [object Object]
         //ram: [object Object]
         //helio: [object Object]
         //trig: [object Object]
         //invin: [object Object]
 
         //console.log(`${model}: ${skins}`);
 
         //iterate through all skins for this model (e.g. all cybi skins)
         for (var j = 0; j < skins.length; j++){
           //array of objects (skins)
           //[{ name: 'Cybi 2', id: 786130582 },...]
           //console.log(skins[j]);
           //${NODEURL}/v2/assets/786130883/balances?limit=1000&currency-greater-than=0
       
 
           var resp = request('GET', `${NODEURL}/v2/assets/${skins[j].id}/balances?limit=1000&currency-greater-than=0`, {
             headers: {
               'user-agent': 'example-user-agent',
             },
           });
 
 
           var bodydata = JSON.parse(resp.getBody('utf8'));
           //console.log(bodydata);
      
 

             /*
             balances: [
                 {
                   address: 'AXUMZMKU622SASRE4CCYCJK4EEY2BKABJIWILTGE34BJJZRSPJLGOLANMA',
                   amount: 1,
                   deleted: false,
                   'is-frozen': false,
                   'opted-in-at-round': 22057083
                 },
                 {
                   address: 'CCNFTNFTSXVYGCGP5EU7AMUTQLUZI6TXRS4XMW2LP4XARZH3LB6RZPUOQM',
                   amount: 3,
                   deleted: false,
                   'is-frozen': false,
                   'opted-in-at-round': 21771839
                 },
                 ...
             */

           var holderstemp = [];
           
           for(let wal = 0; wal < bodydata.balances.length ;wal++){
 
             var current = bodydata.balances[wal];
             if(!current.deleted){
               //console.log(current.address);
               holderstemp.push({ _id: current.address, count:current.amount});
               //holderstemp[current.address] = current.amount;
             }
           }
 
           
 
           var alreadypresent = false;
           for(let s = 0; s < nftlist.length ;s++){
             //console.log(nftlist[s]._id + "---"+ skins[j].id); 
             if(nftlist[s]._id == skins[j].id){
               //exisiting one
               alreadypresent = true;
               break;
             }
           }
 
 
           if(alreadypresent){
             //update only holders field
             var update = 
             {
                 holders:holderstemp,
                 updated_date:Date.now()
             };
             var filter = {_id: skins[j].id};
         
             NftSnapshot.findOneAndUpdate(filter, update, {new: true}, function(errx, msgx) {
                 if (errx){
                   //res.send(errx);
                   //console.log(errx);
                 }else{
                   //res.json(nftsnap);
                   //console.log("updatedto->"+msgx);
                 }
               });
 
           }else{
             //enter new document
             var skinsnap = new NftSnapshot({
               _id:skins[j].id,
               rarity:selectedrarity,
               base:model,
               name:skins[j].name,
               holders:holderstemp,
             });
 
             //console.log(skinsnap);;
 
             skinsnap.save(function(errx, msgx) {
               if (errx){
                 //res.send(err);
                 console.log("err"+errx);
                 //will throw err on duplicate key... nothign of importance tbh
               }
               else{
                 //res.json(matchh);
                 //console.log("inserted->"+msgx);
               } 
             });
           }
 
             //console.log(holderstemp);
 
         }//end iterate skins for this champ
 
       }//end iterate champs of this rarity
 
 
     }
 
   //console.log("nft holder snapshots updated");
   res.send("specials holders snapshots updated");
 
   });
 
 };


//UNCOMMONS
//cron function 
exports.updatedbcron = function(req, res) {

  /// console.log(NFTS.UNCOMMON);
 
   //iterat over all models for this rarity
   var selectedrarity = "UNCOMMON";
 
   NftSnapshot.find({rarity:selectedrarity}, function(err, nftlist) {
     if (err){
       //res.send(err);
     }else{
 
       for (const [model, skins] of Object.entries(NFTS[selectedrarity])) {


         //iterate through all skins for this model (e.g. all cybi skins)
         for (var j = 0; j < skins.length; j++){
 
 
           var resp = request('GET', `${NODEURL}/v2/assets/${skins[j].id}/balances?limit=1000&currency-greater-than=0`, {
             headers: {
               'user-agent': 'example-user-agent',
             },
           });
 
           var bodydata = JSON.parse(resp.getBody('utf8'));
           //console.log(bodydata);
 

           var holderstemp = [];
           
           for(let wal = 0; wal < bodydata.balances.length ;wal++){
 
             var current = bodydata.balances[wal];
             if(!current.deleted){
               //console.log(current.address);
               holderstemp.push({ _id: current.address, count:current.amount});
               //holderstemp[current.address] = current.amount;
             }
           }
 
           
 
           var alreadypresent = false;
           for(let s = 0; s < nftlist.length ;s++){
             //console.log(nftlist[s]._id + "---"+ skins[j].id); 
             if(nftlist[s]._id == skins[j].id){
               //exisiting one
               alreadypresent = true;
               break;
             }
           }
 
 
           if(alreadypresent){
             //update only holders field
             var update = 
             {
                 holders:holderstemp,
                 updated_date:Date.now()
             };
             var filter = {_id: skins[j].id};
         
             NftSnapshot.findOneAndUpdate(filter, update, {new: true}, function(errx, msgx) {
                 if (errx){
                   //res.send(errx);
                   //console.log(errx);
                 }else{
                   //res.json(nftsnap);
                   //console.log("updatedto->"+msgx);
                 }
               });
 
           }else{
             //enter new document
             var skinsnap = new NftSnapshot({
               _id:skins[j].id,
               rarity:selectedrarity,
               base:model,
               name:skins[j].name,
               holders:holderstemp,
             });
 
             //console.log(skinsnap);;
 
             skinsnap.save(function(errx, msgx) {
               if (errx){
                 //res.send(err);
                 console.log("err"+errx);
                 //will throw err on duplicate key... nothign of importance tbh
               }
               else{
                 //res.json(matchh);
                 //console.log("inserted->"+msgx);
               } 
             });
           }
 
             //console.log(holderstemp);
 
         }//end iterate skins for this champ
 
       }//end iterate champs of this rarity
 
 
     }
 
   //console.log("nft holder snapshots updated");
   //res.send("nft holder snapshots updated");
 
   });
 
 };

//manual call for uncommons
exports.updatedb = function(req, res) {

 /// console.log(NFTS.UNCOMMON);

  //iterat over all models for this rarity
  var selectedrarity = "UNCOMMON";

  NftSnapshot.find({rarity:selectedrarity}, function(err, nftlist) {
    if (err){
      //res.send(err);
    }else{

      for (const [model, skins] of Object.entries(NFTS[selectedrarity])) {
        //cybi: [object Object]
        //tertius: [object Object]
        //coyote: [object Object]
        //hammer: [object Object]
        //ram: [object Object]
        //helio: [object Object]
        //trig: [object Object]
        //invin: [object Object]

        //console.log(`${model}: ${skins}`);

        //iterate through all skins for this model (e.g. all cybi skins)
        for (var j = 0; j < skins.length; j++){
          //array of objects (skins)
          //[{ name: 'Cybi 2', id: 786130582 },...]
          ///console.log(skins[j]);
          //${NODEURL}/v2/assets/786130883/balances?limit=1000&currency-greater-than=0
          

          var resp = request('GET', `${NODEURL}/v2/assets/${skins[j].id}/balances?limit=1000&currency-greater-than=0`, {
            headers: {
              'user-agent': 'example-user-agent',
            },
          });

          var bodydata = JSON.parse(resp.getBody('utf8'));
          //console.log(bodydata);


          var holderstemp = [];
          
          for(let wal = 0; wal < bodydata.balances.length ;wal++){

            var current = bodydata.balances[wal];
            if(!current.deleted){
              //console.log(current.address);
              holderstemp.push({ _id: current.address, count:current.amount});
              //holderstemp[current.address] = current.amount;
            }
          }

          

          var alreadypresent = false;
          for(let s = 0; s < nftlist.length ;s++){
            //console.log(nftlist[s]._id + "---"+ skins[j].id); 
            if(nftlist[s]._id == skins[j].id){
              //exisiting one
              alreadypresent = true;
              break;
            }
          }


          if(alreadypresent){
            //update only holders field
            var update = 
            {
                holders:holderstemp,
                updated_date:Date.now()
            };
            var filter = {_id: skins[j].id};
        
            NftSnapshot.findOneAndUpdate(filter, update, {new: true}, function(errx, msgx) {
                if (errx){
                  //res.send(errx);
                  //console.log(errx);
                }else{
                  //res.json(nftsnap);
                  //console.log("updatedto->"+msgx);
                }
              });

          }else{
            //enter new document
            var skinsnap = new NftSnapshot({
              _id:skins[j].id,
              rarity:selectedrarity,
              base:model,
              name:skins[j].name,
              holders:holderstemp,
            });

            //console.log(skinsnap);;

            skinsnap.save(function(errx, msgx) {
              if (errx){
                //res.send(err);
                console.log("err"+errx);
                //will throw err on duplicate key... nothign of importance tbh
              }
              else{
                //res.json(matchh);
                //console.log("inserted->"+msgx);
              } 
            });
          }

            //console.log(holderstemp);

        }//end iterate skins for this champ

      }//end iterate champs of this rarity


    }

  console.log("nft holder snapshots updated");
  //res.send("nft holder snapshots updated");

  });

};


exports.getInventory = async function(req, res) {

  //var walletaddress = "CCNFTNFTSXVYGCGP5EU7AMUTQLUZI6TXRS4XMW2LP4XARZH3LB6RZPUOQM";
  var walletaddress = req.params.walletId;


  if(walletaddress == "PVEBOTADDRESS"){
    //add NFTs without extra boost calcuclation
    //it respects already assigned boost
    res.json(NFTS.PVEBOT);
    return;
  }else if(walletaddress == "CCDEV23KIZIJKRSUMYXSIC3KTLCHXYJAOSRJTDCNYJCE7SQFU4X3T5XX7I"){
    //add NFTS, then calculate boost
    res.json(BOOSTER.AddBoost(NFTS.SIMON));
    return;
  }
  else if(walletaddress == "ALGOGEMSBOTCOSMICCHAMPS27J3TUZ3CRYR5LB44XAN2RB6M5HLN7ZHJMI"){
    //prize bot
    res.json(NFTS.GEMSBOT);
    return;
  }

  //aggregate query to produce desired format
  //[{_id:tertius_skinid,quantity:x},{_id:tertius_skinid,quantity:y},{_id:cybi_skinid,quantity:x}...]
  NftSnapshot.aggregate([
    {
      '$match': {
        'holders._id': walletaddress
      }
    }, {
      '$project': {
        'holdertemp': {
          '$filter': {
            'input': '$holders', 
            'as': 'holder', 
            'cond': {
              '$eq': [
                '$$holder._id', walletaddress
              ]
            }
          }
        }, 
        '_id': 1,
        'base':1,
        'name':1
      }
    }/*, {
      '$project': {
        '_id': 1, 
        'base': 1, 
        'quantity': {
          '$first': '$holdertemp.count'
        }
      }
    }*/
  ],function(errx, docsx) {

    var buildresponse = [];


    var diiferentfireballcount = 0;

    
    if(errx){
      console.log("error fetching inventory for:"+walletaddress);
    }else{
      //console.log(docsx);
      //no need to check really...
      //var len = ((await agg).length);
      for (var doc of docsx) {


        if(doc.base == "spacerock"){
          //careful, in this case we actually replace the spacerodks actual id with spacerock_1,2,3,4...
          //to acommondate each months needs
          //you need to MODIFY gen1nftlist.js / constants to reflect that every time you introduce new spacerocks
          buildresponse.push({
            '_id' : doc.name,
            /*'quantity':doc.quantity*/
            'quantity':doc.holdertemp[0].count
          });
        }else if(doc.base == "fireball"){
          //manipulate frieballs for now, replace names
          
          if(doc._id == "2411764415"){
            //fel
            diiferentfireballcount++;
            buildresponse.push({
              '_id' : 'fireball_100000000',
              /*'quantity':doc.quantity*/
              'quantity':doc.holdertemp[0].count
            });
          }else if(doc._id == "2411764596"){
            //void
            diiferentfireballcount++;
            buildresponse.push({
              '_id' : 'fireball_600000000',
              /*'quantity':doc.quantity*/
              'quantity':doc.holdertemp[0].count
            });
          }else if(doc._id == "2411765359"){
            //midnight
            diiferentfireballcount++;
            buildresponse.push({
              '_id' : 'fireball_500000000',
              /*'quantity':doc.quantity*/
              'quantity':doc.holdertemp[0].count
            });
          }else if(doc._id == "2411764715"){
            //blau
            diiferentfireballcount++;
            buildresponse.push({
              '_id' : 'fireball_200000000',
              /*'quantity':doc.quantity*/
              'quantity':doc.holdertemp[0].count
            });
          }else if(doc._id == "2411765749"){
            //scarlet
            diiferentfireballcount++;
            buildresponse.push({
              '_id' : 'fireball_300000000',
              /*'quantity':doc.quantity*/
              'quantity':doc.holdertemp[0].count
            });
          }else if(doc._id == "2411765647"){
            //golden
            diiferentfireballcount++;
            buildresponse.push({
              '_id' : 'fireball_400000000',
              /*'quantity':doc.quantity*/
              'quantity':doc.holdertemp[0].count
            });
          }else if(doc._id == "2411818661"){
            //ultimate
            diiferentfireballcount++;
            buildresponse.push({
              '_id' : 'fireball_700000000',
              /*'quantity':doc.quantity*/
              'quantity':doc.holdertemp[0].count
            });
          }

        }else{
          buildresponse.push({
              '_id' : doc.base+'_'+doc._id,
              /*'quantity':doc.quantity*/
              'quantity':doc.holdertemp[0].count
          });
        }


      }

      //special bonus for fireballs
      if(diiferentfireballcount > 5){
        //if we have all 6 different fireballs (scenario is that we have 7 actually ^^ in thsi case we would have to increment the ultimate fireball count..., ignore, as it's not natural happening case)
        //ADD EXTRA ULTIMATE FIREBALL ENTRY
        buildresponse.push({
          '_id' : 'fireball_700000000',
          /*'quantity':doc.quantity*/
          'quantity':1
        });
      }

    }
    //console.log(buildresponse); 


    res.json(BOOSTER.AddBoost(buildresponse));
    //res.json(buildresponse);

  });


};


exports.getInventoryTest = async function(req, res) {

  var walletaddress = "CCNFTNFTSXVYGCGP5EU7AMUTQLUZI6TXRS4XMW2LP4XARZH3LB6RZPUOQM";
  //var walletaddress = req.params.walletId;

  //nice little aggregate query to produce 
  //[{_id:tertius_skinid,quantity:x},{_id:tertius_skinid,quantity:y},{_id:cybi_skinid,quantity:x}...]
  NftSnapshot.aggregate([
    {
      '$match': {
        'holders._id': walletaddress
      }
    }, {
      '$project': {
        'holdertemp': {
          '$filter': {
            'input': '$holders', 
            'as': 'holder', 
            'cond': {
              '$eq': [
                '$$holder._id', walletaddress
              ]
            }
          }
        }, 
        '_id': 1,
        'base':1,
        'name':1
      }
    }/*, {
      '$project': {
        '_id': 1, 
        'base': 1, 
        'quantity': {
          '$first': '$holdertemp.count'
        }
      }
    }*/
  ],function(errx, docsx) {

    var buildresponse = [];
    
    if(errx){
      console.log("error fetching inventory for:"+walletaddress);
    }else{
      //console.log(docsx);
      //no need to check really...
      //var len = ((await agg).length);
      for (var doc of docsx) {

        //console.log(doc.holdertemp[0].count);

          if(doc.base == "spacerock"){
            buildresponse.push({
              '_id' : doc.name,
              /*'quantity':doc.quantity*/
              'quantity':doc.holdertemp[0].count
            });
          }else{
            buildresponse.push({
                '_id' : doc.base+'_'+doc._id,
                /*'quantity':doc.quantity*/
                'quantity':doc.holdertemp[0].count
            });
          }
      }
    }
    console.log(buildresponse); 
  });


};


//RARES
//cron function
exports.updateRaresCronPS = function(req, res) {

   //console.log(NFTS.RARE);
   //console.log("yup");
 
   //iterat over all models for this rarity
   var selectedrarity = "RARE";
 
   NftSnapshot.find({rarity:selectedrarity}, function(err, nftlist) {
     if (err){
       //res.send(err);
       //console.log("errrrrrr");
     }else{
        //console.log("here");
       for (const [model, skins] of Object.entries(NFTS[selectedrarity])) {
         //cybi: [object Object]
         //tertius: [object Object]
         //coyote: [object Object]
         //hammer: [object Object]
         //ram: [object Object]
         //helio: [object Object]
         //trig: [object Object]
         //invin: [object Object]
 
         //console.log(`${model}: ${skins}`);
 
         //iterate through all skins for this model (e.g. all cybi skins)
         for (var j = 0; j < skins.length; j++){
           //array of objects (skins)
           //[{ name: 'Cybi 2', id: 786130582 },...]
           ///console.log(skins[j]);
           //${NODEURL}/v2/assets/786130883/balances?limit=1000&currency-greater-than=0
       
 
           var resp = request('GET', `${NODEURL}/v2/assets/${skins[j].id}/balances?limit=1000&currency-greater-than=0`, {
             headers: {
               'user-agent': 'example-user-agent',
             },
           });
 
           var bodydata = JSON.parse(resp.getBody('utf8'));

 

           var holderstemp = [];
           
           for(let wal = 0; wal < bodydata.balances.length ;wal++){
 
             var current = bodydata.balances[wal];
             if(!current.deleted){
               //console.log(current.address);
               holderstemp.push({ _id: current.address, count:current.amount});
               //holderstemp[current.address] = current.amount;
             }
           }
 
           
           var alreadypresent = false;
           for(let s = 0; s < nftlist.length ;s++){
             //console.log(nftlist[s]._id + "---"+ skins[j].id); 
             if(nftlist[s]._id == skins[j].id){
               //exisiting one
               alreadypresent = true;
               break;
             }
           }
 
 
           if(alreadypresent){
             //update only holders field
             var update = 
             {
                 holders:holderstemp,
                 updated_date:Date.now()
             };
             var filter = {_id: skins[j].id};
         
             NftSnapshot.findOneAndUpdate(filter, update, {new: true}, function(errx, msgx) {
                 if (errx){
                   //res.send(errx);
                   console.log(errx);
                 }else{
                   //res.json(nftsnap);
                   //console.log("updatedto->"+msgx);
                 }
               });
 
           }else{
             //enter new document
             var skinsnap = new NftSnapshot({
               _id:skins[j].id,
               rarity:selectedrarity,
               base:model,
               name:skins[j].name,
               holders:holderstemp,
             });
 
             //console.log(skinsnap);;
 
             skinsnap.save(function(errx, msgx) {
               if (errx){
                 //res.send(err);
                 //console.log("err"+errx);
                 //will throw err on duplicate key... nothign of importance tbh
               }
               else{
                 //res.json(matchh);
                 //console.log("inserted->"+msgx);
               } 
             });
           }
 
             //console.log(holderstemp);
 
         }//end iterate skins for this champ
 
       }//end iterate champs of this rarity
 
     }
 
   //console.log("nft holder snapshots updated");
   //res.send("nft holder snapshots updated");
 
   });
 
};


//EPICS
//cron function
exports.updateEpicCronPS = function(req, res) {

  /// console.log(NFTS.UNCOMMON);
 
   //iterat over all models for this rarity
   var selectedrarity = "EPIC";
 
   NftSnapshot.find({rarity:selectedrarity}, function(err, nftlist) {
     if (err){
       //res.send(err);
     }else{
 
       for (const [model, skins] of Object.entries(NFTS[selectedrarity])) {
         //cybi: [object Object]
         //tertius: [object Object]
         //coyote: [object Object]
         //hammer: [object Object]
         //ram: [object Object]
         //helio: [object Object]
         //trig: [object Object]
         //invin: [object Object]
 
         //console.log(`${model}: ${skins}`);
 
         //iterate through all skins for this model (e.g. all cybi skins)
         for (var j = 0; j < skins.length; j++){
           //array of objects (skins)
           //[{ name: 'Cybi 2', id: 786130582 },...]
           ///console.log(skins[j]);
           //${NODEURL}/v2/assets/786130883/balances?limit=1000&currency-greater-than=0
     
 
           var resp = request('GET', `${NODEURL}/v2/assets/${skins[j].id}/balances?limit=1000&currency-greater-than=0`, {
             headers: {
              'user-agent': 'example-user-agent',
             },
           });
 
           var bodydata = JSON.parse(resp.getBody('utf8'));
 
           var holderstemp = [];
           
           for(let wal = 0; wal < bodydata.balances.length ;wal++){
 
             var current = bodydata.balances[wal];
             if(!current.deleted){
               //console.log(current.address);
               holderstemp.push({ _id: current.address, count:current.amount});
               //holderstemp[current.address] = current.amount;
             }
           }
 
           
 
           var alreadypresent = false;
           for(let s = 0; s < nftlist.length ;s++){
             //console.log(nftlist[s]._id + "---"+ skins[j].id); 
             if(nftlist[s]._id == skins[j].id){
               //exisiting one
               alreadypresent = true;
               break;
             }
           }
 
 
           if(alreadypresent){
             //update only holders field
             var update = 
             {
                 holders:holderstemp,
                 updated_date:Date.now()
             };
             var filter = {_id: skins[j].id};
         
             NftSnapshot.findOneAndUpdate(filter, update, {new: true}, function(errx, msgx) {
                 if (errx){
                   //res.send(errx);
                   console.log("E-"+errx);
                 }else{
                   //res.json(nftsnap);
                   //console.log("updatedto->"+msgx);
                   //console.log("updatedto->E");
                 }
               });
 
           }else{
             //enter new document
             var skinsnap = new NftSnapshot({
               _id:skins[j].id,
               rarity:selectedrarity,
               base:model,
               name:skins[j].name,
               holders:holderstemp,
             });
 
             //console.log(skinsnap);;
 
             skinsnap.save(function(errx, msgx) {
               if (errx){
                 //res.send(err);
                 console.log("err"+errx);
                 //will throw err on duplicate key... nothign of importance tbh
               }
               else{
                 //res.json(matchh);
                 //console.log("inserted->"+msgx);
                 //console.log("inserted->E");
               } 
             });
           }
 
             //console.log(holderstemp);
 
         }//end iterate skins for this champ
 
       }//end iterate champs of this rarity
 
 
     }
 
   //console.log("nft holder epi snapshots updated");
   //res.send("nft holder snapshots updated");
 
   });
 
 };

//LEGGIES
//cron function
exports.updateLeggCronPS = function(req, res) {

  /// console.log(NFTS.UNCOMMON);
 
   //iterat over all models for this rarity
   var selectedrarity = "LEGENDARY";
 
   NftSnapshot.find({rarity:selectedrarity}, function(err, nftlist) {
     if (err){
       //res.send(err);
     }else{
 
       for (const [model, skins] of Object.entries(NFTS[selectedrarity])) {

         for (var j = 0; j < skins.length; j++){
 
           var resp = request('GET', `${NODEURL}/v2/assets/${skins[j].id}/balances?limit=1000&currency-greater-than=0`, {
             headers: {
              'user-agent': 'example-user-agent',
             },
           });
 
           var bodydata = JSON.parse(resp.getBody('utf8'));
           //console.log("resp");
           //console.log("next");
           var waitTill = new Date(new Date().getTime() + 150);
           while(waitTill > new Date()){}
 
           var holderstemp = [];
           
           for(let wal = 0; wal < bodydata.balances.length ;wal++){
 
             var current = bodydata.balances[wal];
             if(!current.deleted){
               //console.log(current.address);
               holderstemp.push({ _id: current.address, count:current.amount});
               //holderstemp[current.address] = current.amount;
             }
           }
 
           
 
           var alreadypresent = false;
           for(let s = 0; s < nftlist.length ;s++){
             //console.log(nftlist[s]._id + "---"+ skins[j].id); 
             if(nftlist[s]._id == skins[j].id){
               //exisiting one
               alreadypresent = true;
               break;
             }
           }
 
 
           if(alreadypresent){
             //update only holders field
             var update = 
             {
                 holders:holderstemp,
                 updated_date:Date.now()
             };
             var filter = {_id: skins[j].id};
         
             NftSnapshot.findOneAndUpdate(filter, update, {new: true}, function(errx, msgx) {
                 if (errx){
                   //res.send(errx);
                   console.log("L-"+errx);
                 }else{
                   //res.json(nftsnap);
                   //console.log("updatedto->"+msgx);
                   //console.log("updatedto->L");
                 }
               });
 
           }else{
             //enter new document
             var skinsnap = new NftSnapshot({
               _id:skins[j].id,
               rarity:selectedrarity,
               base:model,
               name:skins[j].name,
               holders:holderstemp,
             });
 
             //console.log(skinsnap);;
 
             skinsnap.save(function(errx, msgx) {
               if (errx){
                 //res.send(err);
                 console.log("err"+errx);
                 //will throw err on duplicate key... nothign of importance tbh
               }
               else{
                 //res.json(matchh);
                 //console.log("inserted->"+msgx);
                 //console.log("inserted->L");
               } 
             });
           }
 
             //console.log(holderstemp);
 
         }//end iterate skins for this champ
 
       }//end iterate champs of this rarity
 
 
     }

 
   });
 
 };
