import algosdk from 'algosdk';
var senderMnemonic = process.env.SenderMnemonic;

//above here switch based on env, local or aws
///////////////////////////////////////////////////////////////

var senderAccount = algosdk.mnemonicToSecretKey(senderMnemonic);
const senderAddress = senderAccount.addr;
//console.log("addr"+senderAddress);

const algodToken = '';
const algodServer = 'https://mainnet-api.algonode.cloud';
const algodPort = 443;


//if there is an error, we'll send note to this address
const errorReportingAddress = "ERRORREPORTINGADDRESS";


const enc = new TextEncoder();


//console.log("test");

let algodclient = new algosdk.Algodv2(algodToken, algodServer, algodPort);

///replace when in aws
export const handler = async (event) => {

  //console.log("start me");
  //console.log(JSON.stringify(event));

//repl
///(async () => {
    //end repl

 //when wrokign with SQS integration to lambda, no need to interact with sqs manually, jsut hadnle events and that's that
 //if lambda returns error, then message won't get deleted from queue
 //handle error cases and that's that, else it will simply retry stuff based on queue msg lifespan setting
 
    console.log("start processing event records");
    for (const message of event.Records) {
        await processMessageAsync(message);
    }
    console.info("done");
 

  
///replace when in aws    
}

//repl
//})().catch(e => {
//    console.log(e);
//    console.trace();
//});
//end repl

var serialize = function(object) {
    return JSON.stringify(object, null, 2)
}


// Function Borrowed from Algorand Inc.
//confirmation functions basically
const waitForConfirmation = async function (algodClient, txId) {
    let lastround = (await algodClient.status().do())['last-round'];
     while (true) {
        const pendingInfo = await algodClient.pendingTransactionInformation(txId).do();
        if (pendingInfo['confirmed-round'] !== null && pendingInfo['confirmed-round'] > 0) {
          //Got the completed Transaction
          console.log('Transaction '+txId+' confirmed in round ' + pendingInfo['confirmed-round']);
          /*try {
           fs.writeFileSync(`${writetodoc}`,`Transaction ${txId} confirmed in round ${pendingInfo['confirmed-round']}\n`,{
             encoding: "utf8",
             flag: "a+"
           });
         } catch (err) {
           console.error(err)
         }  */ 
          break;
        }
        lastround++;
        await algodClient.statusAfterBlock(lastround).do();
     }
 };
 
 

async function processMessageAsync(message) {
  try {
    
    //console.log("parse msg");
    //console.log(message);
    
    var payload = JSON.parse(message.body);

    console.log(payload);

    let status = await algodclient.status().do();
    //console.log("Algorand network status: %o", status);
    
    let accountInfo = await algodclient.accountInformation(senderAddress).do();
    if(accountInfo.amount < 1000000){
        var reminderMsg = "###ALERT   ###ALERT   ###ALERT  ALGO TOP UP NEEDED!!! ON "+senderAddress;
        console.log(reminderMsg);
        //send topup needed alert to dev wallet so reminder is there
        let remindernote = enc.encode(reminderMsg);
        let params = await algodclient.getTransactionParams().do();
        let txn = algosdk.makePaymentTxnWithSuggestedParams(senderAddress, errorReportingAddress, 0, undefined,remindernote, params);
        let RawSignedTxn = txn.signTxn(senderAccount.sk);
        let txId = txn.txID().toString(); //can either write txid to db, or simply wait and write once confirmed(although that will keep process running for about 7 more seconds)
        //console.log("Signed transaction with txID: %s", txId);
        await algodclient.sendRawTransaction(RawSignedTxn).do();
        return;
    }
    //console.log("Account balance for sender: %d microAlgos", accountInfo.amount);

    //exit if invalid winner wallet address
    var winnerAddressCheck = payload['winnerWalletId'];
    var invalidChar = '0';
    var testresult = winnerAddressCheck.startsWith(invalidChar);
    //console.log("testresult-"+testresult+' --'+winnerAddressCheck);
    if(testresult){
        var reminderMsgTest = "###ALERT   ###ALERT   ###ALERT  INVALID ADDRESS!!! ON "+winnerAddressCheck;
        console.log(reminderMsgTest);
        //send topup needed alert to dev wallet so reminder is there
        let remindernote = enc.encode(reminderMsgTest);
        let params = await algodclient.getTransactionParams().do();
        let txn = algosdk.makePaymentTxnWithSuggestedParams(senderAddress, errorReportingAddress, 0, undefined,remindernote, params);
        let RawSignedTxn = txn.signTxn(senderAccount.sk);
        let txId = txn.txID().toString(); //can either write txid to db, or simply wait and write once confirmed(although that will keep process running for about 7 more seconds)
        //console.log("Signed transaction with txID: %s", txId);
        await algodclient.sendRawTransaction(RawSignedTxn).do();
        return;
    }

    //exit if invalid loser wallet address
    var loserAddressCheck = payload['winnerWalletId'];
    var testresultx = loserAddressCheck.startsWith(invalidChar);
    //console.log("testresult-"+testresult+' --'+winnerAddressCheck);
    if(testresultx){
        var reminderMsgTest = "###ALERT   ###ALERT   ###ALERT  INVALID ADDRESS!!! ON "+loserAddressCheck;
        console.log(reminderMsgTest);
        //send topup needed alert to dev wallet so reminder is there
        let remindernote = enc.encode(reminderMsgTest);
        let params = await algodclient.getTransactionParams().do();
        let txn = algosdk.makePaymentTxnWithSuggestedParams(senderAddress, errorReportingAddress, 0, undefined,remindernote, params);
        let RawSignedTxn = txn.signTxn(senderAccount.sk);
        let txId = txn.txID().toString(); //can either write txid to db, or simply wait and write once confirmed(although that will keep process running for about 7 more seconds)
        //console.log("Signed transaction with txID: %s", txId);
        await algodclient.sendRawTransaction(RawSignedTxn).do();
        return;
    }


        //prepare json object to be used in note
        var pushItem = {};                    
        pushItem['recordId'] = payload['recordId'];


        //leave this in bcz it was added for some reason i guess...
        var winnerNickname = "undefinedNickname";
        //sanity check
        if(payload['winnerNickname']){
            winnerNickname = payload['winnerNickname'];
        }
        var loserNickname = "undefinedNickname";
        //sanity check
        if(payload['loserNickname']){
            loserNickname = payload['loserNickname'];
        }
      


        pushItem['winnerNickname'] = winnerNickname;
        pushItem['winnerRating'] = payload['winnerRating'];
        pushItem['winnerTier'] = payload['winnerHUD'];
        pushItem['winnerWallet'] = payload['winnerWalletId'];

        pushItem['loserNickname'] = loserNickname;
        pushItem['loserRating'] = payload['loserRating'];
        pushItem['loserTier'] = payload['loserHUD'];
        pushItem['loserWallet'] = payload['loserWalletId'];

        pushItem['matchResult'] = payload['Result'];

        
        var notemsg = serialize(pushItem);
        var note = enc.encode(notemsg);
        //console.log(notemsg);

        let params = await algodclient.getTransactionParams().do();
        let txn = algosdk.makePaymentTxnWithSuggestedParams(senderAddress, senderAddress, 1000000, undefined,note, params);
        let RawSignedTxn = txn.signTxn(senderAccount.sk);
        let txId = txn.txID().toString(); //can either write txid to db, or simply wait and write once confirmed(although that will keep process running for about 7 more seconds)
        console.log("Signed transaction with txID: %s", txId);
        await algodclient.sendRawTransaction(RawSignedTxn).do();
        
        return;
    


///////////////////////////////////        
    

  } catch (err) {
    console.error("An error occurred xx");
    console.log(message);
    throw err;
  }
}