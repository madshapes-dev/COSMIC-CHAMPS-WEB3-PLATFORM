import algosdk from 'algosdk';
import { DynamoDBClient } from "@aws-sdk/client-dynamodb";
import { DynamoDBDocumentClient, UpdateCommand } from "@aws-sdk/lib-dynamodb";

const client = new DynamoDBClient({});
const docClient = DynamoDBDocumentClient.from(client);
const TABLE_NAME = "MatchReport";

var senderMnemonic = process.env.SenderMnemonic;

//above here switch based on env, local or aws
///////////////////////////////////////////////////////////////

var senderAccount = algosdk.mnemonicToSecretKey(senderMnemonic);
const senderAddress = senderAccount.addr;

const algodToken = '';
const algodServer = 'https://mainnet-api.algonode.cloud';
const algodPort = 443;



const errorReportingAddress = "ERRORREPORTINGADDRESS";


const payPlanetFee = true;
//send portion of rewards to this wallet
const planetWalletAddress = "PLANETDEPOSITWALLETADDRESS";
//if we pay 1x to the player, we pay additional x*planetsharepercent to the planet wallet
const planetSharePercent = 0.20;
const planetOwnersCTA= "Planet owners collect portion of the rewards paid out in the battle Arena. Read more about the planets: TBA\n\n";



//amounts are in microalgos or in number x decimals
const BOT_SETTINGS = {


    /*
    //how would algo paying bot look like below
    "botid1":{
        "Title":"Prize Bot",
        "ASA":0,
        "AsaName":"ALGO",
        "Decimals": 1000000, 
        "BaseAmount": 0.2, 
        "MessageBody": "You've defeated the Cosmic Champs Prize Bot!",
        "CTA": "Remember, Champ NFTs are stronger in game!\nGet your Champ NFT at: https://www.algogems.io/shops/cosmicChamps/sales\n\n",
        "MessageEnd": "~ Team Cosmic Champs",
    },
    */
    
        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE7":{
            "Title":"Prize Bot",
            "ASA":1065092715,
            "AsaName":"COSG",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":46.32, /* 4mm microcosg = 4 cosg */
            "MessageBody": "You've defeated the Cosmic Bot!",
            "CTA": "Remember, Champ NFTs are stronger in game!\nGet your Champ NFT at: https://www.asalytic.app/collection/cosmic-champs\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },
        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE8":{
            "Title":"Prize Bot",
            "ASA":1065092715,
            "AsaName":"COSG",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":46.32, /* 4mm microcosg = 4 cosg */
            "MessageBody": "You've defeated the Cosmic Bot I!",
            "CTA": "Remember, Champ NFTs are stronger in game!\nGet your Champ NFT at: https://www.asalytic.app/collection/cosmic-champs\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },
        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE9":{
            "Title":"Prize Bot",
            "ASA":1065092715,
            "AsaName":"COSG",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":46.32, /* 4mm microcosg = 4 cosg */
            "MessageBody": "You've defeated the Cosmic Bot II!",
            "CTA": "Remember, Champ NFTs are stronger in game!\nGet your Champ NFT at: https://www.asalytic.app/collection/cosmic-champs\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },
        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE10":{
            "Title":"Prize Bot",
            "ASA":1065092715,
            "AsaName":"COSG",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":46.32, /* 4mm microcosg = 4 cosg */
            "MessageBody": "You've defeated the Cosmic Bot III!",
            "CTA": "Remember, Champ NFTs are stronger in game!\nGet your Champ NFT at: https://www.asalytic.app/collection/cosmic-champs\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },
        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE11":{
            "Title":"Prize Bot",
            "ASA":1065092715,
            "AsaName":"COSG",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":46.32, /* 4mm microcosg = 4 cosg */
            "MessageBody": "You've defeated the Cosmic Bot IV!",
            "CTA": "Remember, Champ NFTs are stronger in game!\nGet your Champ NFT at: https://www.asalytic.app/collection/cosmic-champs\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },


        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE12":{
            "Title":"AlgoGems Bot",
            "ASA":230946361,
            "AsaName":"GEMS",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":5.97, /* roughly 0.1 A*/
            "MessageBody": "You've defeated the AlgoGems Bot!",
            "CTA": "AlgoGems is the OG marketplace on Algorand! Check out wide variety of NFTs and compete for rebates and Algo rewards on all eligible trades.\nBrowse the marketplace at: https://www.algogems.io\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },
        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE13":{
            "Title":"AlgoGems Bot",
            "ASA":230946361,
            "AsaName":"GEMS",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":5.97, /* roughly 0.1 A*/
            "MessageBody": "You've defeated the AlgoGems Bot I!",
            "CTA": "AlgoGems is the OG marketplace on Algorand! Check out wide variety of NFTs and compete for rebates and Algo rewards on all eligible trades.\nBrowse the marketplace at: https://www.algogems.io\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },
        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE14":{
            "Title":"AlgoGems Bot",
            "ASA":230946361,
            "AsaName":"GEMS",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":5.97, /* roughly 0.1 A*/
            "MessageBody": "You've defeated the AlgoGems Bot II!",
            "CTA": "AlgoGems is the OG marketplace on Algorand! Check out wide variety of NFTs and compete for rebates and Algo rewards on all eligible trades.\nBrowse the marketplace at: https://www.algogems.io\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },
        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE15":{
            "Title":"AlgoGems Bot",
            "ASA":230946361,
            "AsaName":"GEMS",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":5.97, /* roughly 0.1 A*/
            "MessageBody": "You've defeated the AlgoGems Bot III!",
            "CTA": "AlgoGems is the OG marketplace on Algorand! Check out wide variety of NFTs and compete for rebates and Algo rewards on all eligible trades.\nBrowse the marketplace at: https://www.algogems.io\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },
        "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE16":{
            "Title":"AlgoGems Bot",
            "ASA":230946361,
            "AsaName":"GEMS",
            "Decimals": 1000000, /* 6 */
            "BaseAmount":5.97, /* roughly 0.1 A*/
            "MessageBody": "You've defeated the AlgoGems Bot IV!",
            "CTA": "AlgoGems is the OG marketplace on Algorand! Check out wide variety of NFTs and compete for rebates and Algo rewards on all eligible trades.\nBrowse the marketplace at: https://www.algogems.io\n\n",
            "MessageEnd": "~ Team Cosmic Champs",
        },

        /*other bots*/

}

const enc = new TextEncoder();


//console.log("test");

let algodclient = new algosdk.Algodv2(algodToken, algodServer, algodPort);

///replace when in aws
export const handler = async (event) => {

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
    docClient.destroy(); // no-op, no clue if needed
    client.destroy(); // destroys DynamoDBClient
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
 
 
 
 
async function writeTxAsync(dbentryid,resultmsg){
     var command = new UpdateCommand({
        TableName: TABLE_NAME,
        Key: {
          Id: dbentryid,
        },
        UpdateExpression: 'set ProcessingResult = :r',
        ExpressionAttributeValues: {
          ':r': resultmsg,
        },
        ReturnValues: "UPDATED_NEW",
      });
    
    var response = await docClient.send(command);
    return;
}
 
 
async function processMessageAsync(message) {
  try {
    
    //console.log(message);
    
    var payload = JSON.parse(message.body);

    //console.log(payload);
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
        await writeTxAsync(payload['recordId'],"algo topup needed");
        return;
    }
    //console.log("Account balance for sender: %d microAlgos", accountInfo.amount);

    //exit if invalid winner wallet address
    var winnerAddressCheck = payload['winnerWalletId'];
    var invalidChar = '0';
    var testresult = winnerAddressCheck.startsWith(invalidChar);
    //console.log("testresult-"+testresult+' --'+winnerAddressCheck);
    if(testresult){
        var reminderMsgTest = "###ALERT   ###ALERT   ###ALERT  INVALID WINNER ADDRESS!!! ON "+winnerAddressCheck;
        console.log(reminderMsgTest);
        //send topup needed alert to dev wallet so reminder is there
        let remindernote = enc.encode(reminderMsgTest);
        let params = await algodclient.getTransactionParams().do();
        let txn = algosdk.makePaymentTxnWithSuggestedParams(senderAddress, errorReportingAddress, 0, undefined,remindernote, params);
        let RawSignedTxn = txn.signTxn(senderAccount.sk);
        let txId = txn.txID().toString(); //can either write txid to db, or simply wait and write once confirmed(although that will keep process running for about 7 more seconds)
        //console.log("Signed transaction with txID: %s", txId);
        await algodclient.sendRawTransaction(RawSignedTxn).do();
        await writeTxAsync(payload['recordId'],"invalid winner address");
        return;
    }


    var botid = payload['loserId'];


    if(BOT_SETTINGS[botid]){
        console.log("available bot entry")
        var selectedbot = BOT_SETTINGS[botid];
        var assetID = selectedbot['ASA'];

        var winnerNickname = "undefinedNickname";
        //sanity check
        if(payload['winnerNickname']){
            winnerNickname = payload['winnerNickname'];
        }

        //modifier adjsuted based on the hud skin(e.g. holder tier)
        //make sure to have double digits everywhere!!! to prevent issus with rounding
        var rewardModifier = 1.00;
        var winnerHUD = "EMPTY";
        //sanity check, but should already be present in the event
        if(payload["winnerHUD"]){
            winnerHUD = payload["winnerHUD"];
        }


        var boostmsg = "**COSG holder tier not detected... no bonus rewards.**\nRead about holder tiers: https://medium.com/cosmic-champs/explained-cosg-holder-tier-rewards-c02f00632475";
        if(winnerHUD != "EMPTY"){
            boostmsg = "**COSG holder tier detected... rewards are increased!!";

            //make sure to have double digits everywhere!!! to prevent issus with rounding
            if(winnerHUD == "bronze"){
                rewardModifier = 1.05;
                boostmsg += " (bronze tier +5%)**";
            }else if(winnerHUD == "silver"){
                rewardModifier = 1.10;
                boostmsg += " (silver tier +10%)**";
            }else if(winnerHUD == "gold"){
                rewardModifier = 1.25;
                boostmsg += " (gold tier +25%)**";
            }else if(winnerHUD == "diamond"){
                rewardModifier = 1.50;
                boostmsg += " (diamond tier +50%)**";
            }
        }

        //rating always exists obviously
        var winnerRating = payload["winnerRating"];
        if(winnerRating >= 1600){
            boostmsg += "\n**High MMR detected (1600+)...rewards are increased!! (+15%)**";
            rewardModifier += 0.15;
        }else if(winnerRating >= 1500){
            boostmsg += "\n**High MMR detected (1500+)...rewards are increased!! (+10%)**";
            rewardModifier += 0.10;
        }else if(winnerRating >= 1400){
            boostmsg += "\n**High MMR detected (1400+)...rewards are increased!! (+5%)**";
            rewardModifier += 0.05;
        }
        
        //add lil extra boost to rewards, or keep at 1.00
        var extraRewardBoost = 1.20;

        var amountToSend = (selectedbot['BaseAmount']* selectedbot['Decimals'] * rewardModifier * extraRewardBoost).toFixed(3);
        //console.log(amountToSend);
        amountToSend = Math.trunc(amountToSend); //sometimes some weird shit happens, get rid of decimal palces
        //console.log(amountToSend);

        //calc the fee base don percentage of total payout
        var planetFeeToSend = (planetSharePercent*amountToSend).toFixed(3);
        planetFeeToSend = Math.trunc(planetFeeToSend);
        if(!payPlanetFee){
            planetFeeToSend = 0;
        }
        //console.log(planetFeeToSend);
        
        //temp one durign test phase
        var testwalletidentifier = payload['winnerWalletId'];
        //replace receiveraddress with actual payload['winnerWalletId']; when done testing, and remove the walletidentifier from above
        
        var receiverAddress = payload['winnerWalletId'];


        var noOpMsg = "###Your wallet is not opted-in to "+selectedbot['AsaName']+"("+assetID+"). Reward forfeited, please opt-in to be able to receive your rewards in the future!###\n\n";
        var notemsg = "Victory!\n"+selectedbot['MessageBody']+"\n\n"+boostmsg+"\nTotal rewards: "+amountToSend/selectedbot['Decimals']+" "+selectedbot['AsaName']+"!\n\n"+selectedbot['CTA']+selectedbot['MessageEnd'];
        var planetFeeMsg = "Player "+winnerNickname+" defeated prize bot!\n"+planetFeeToSend/selectedbot['Decimals']+" "+selectedbot['AsaName']+" added to the planet owners rewards!\n\n"+planetOwnersCTA;
        var note = enc.encode(notemsg);

        //if sending algos handle differently
        if(assetID == 0){
            console.log("sending algos");


            //check if enough balance to send reward!!!!
            
            let accountInfo = await algodclient.accountInformation(senderAddress).do();
            if(accountInfo.amount < (amountToSend + planetFeeToSend + 3000)){
                var reminderMsg = "###ALERT   ###ALERT   ###ALERT  INSUFFICIENT ALGO FOR PAYOUT!!! ON "+senderAddress;
                console.log(reminderMsg);
                //send topup needed alert to dev wallet so reminder is there
                let remindernote = enc.encode(reminderMsg);
                let params = await algodclient.getTransactionParams().do();
                let txn = algosdk.makePaymentTxnWithSuggestedParams(senderAddress, errorReportingAddress, 0, undefined,remindernote, params);
                let RawSignedTxn = txn.signTxn(senderAccount.sk);
                let txId = txn.txID().toString(); //can either write txid to db, or simply wait and write once confirmed(although that will keep process running for about 7 more seconds)
                //console.log("Signed transaction with txID: %s", txId);
                await algodclient.sendRawTransaction(RawSignedTxn).do();
                await writeTxAsync(payload['recordId'],"insufficient algo for payout");
                return;
            }

 
            let params = await algodclient.getTransactionParams().do();
            let txn = algosdk.makePaymentTxnWithSuggestedParams(senderAddress, receiverAddress, amountToSend, undefined,note, params);
            let RawSignedTxn = txn.signTxn(senderAccount.sk);
            let txId = txn.txID().toString(); //can either write txid to db, or simply wait and write once confirmed(although that will keep process running for about 7 more seconds)
            console.log("Signed transaction with txID: %s", txId);
            await algodclient.sendRawTransaction(RawSignedTxn).do();
            await writeTxAsync(payload['recordId'],txId);
            console.log(notemsg);
            //no need to wait for confirmation tbh, 10k tps possible in algo, if this fails fuck it
            //await waitForConfirmation(algodclient,txId,2);


            //handle planet owners fee payout (algo)
            //tbd


        }else{
            console.log("sending ASA");

            var hasenoughbalance = false;
            //check if enough balance to send reward :)
            let checksender = await algodclient.accountInformation(senderAddress).do();
            for (let idx = 0; idx < checksender['assets'].length; idx++) {
                let scrutinizedAsset = checksender['assets'][idx];
                if (scrutinizedAsset['asset-id'] == assetID) {
                    let myassetholding = JSON.stringify(scrutinizedAsset, undefined, 2);
                    //console.log("current balance of asa:"+scrutinizedAsset.amount);
                    //console.log("assetholdinginfo = " + myassetholding);
                    if(scrutinizedAsset.amount >= amountToSend + planetFeeToSend){
                        hasenoughbalance = true;
                        //console.log("enough balance to pay");
                        //console.log(scrutinizedAsset.amount);
                    }
                }
            }

            if(!hasenoughbalance){
                var reminderMsg = "###ALERT   ###ALERT   ###ALERT  INSUFFICIENT ASA ("+assetID+") BALANCE FOR PAYOUT!!! ON "+senderAddress;
                //send topup needed alert to dev wallet so reminder is there
                let remindernote = enc.encode(reminderMsg);
                let params = await algodclient.getTransactionParams().do();
                let txn = algosdk.makePaymentTxnWithSuggestedParams(senderAddress, errorReportingAddress, 0, undefined,remindernote, params);
                let RawSignedTxn = txn.signTxn(senderAccount.sk);
                let txId = txn.txID().toString(); //can either write txid to db, or simply wait and write once confirmed(although that will keep process running for about 7 more seconds)
                //console.log("Signed transaction with txID: %s", txId);
                await algodclient.sendRawTransaction(RawSignedTxn).do();
                await writeTxAsync(payload['recordId'],"insufficient ASA for payout");
                console.log(reminderMsg);
                return;
            }


            //check if user opted in

            //console.log("balance of reciever");
            //await printAssetHolding(algodclient, receiverAddress, assetID);
            let checkaccount = await algodclient.accountInformation(receiverAddress).do();
            let optedin = false;
            for (let idx = 0; idx < checkaccount['assets'].length; idx++) {
                let scrutinizedAsset = checkaccount['assets'][idx];
                if (scrutinizedAsset['asset-id'] == assetID) {
                    let myassetholding = JSON.stringify(scrutinizedAsset, undefined, 2);
                    //console.log("current balance of asa:"+scrutinizedAsset.amount);
                    //console.log("assetholdinginfo = " + myassetholding);
                    optedin = true;
                }
            }

            if(optedin){
                console.log("opted in asa "+assetID);
 
                let params = await algodclient.getTransactionParams().do();
                let txn = algosdk.makeAssetTransferTxnWithSuggestedParams(senderAddress, receiverAddress, undefined, undefined, amountToSend,  note, assetID, params);
                let RawSignedTxn = txn.signTxn(senderAccount.sk);
                let txId = txn.txID().toString();
                console.log("Signed transaction with txID: %s", txId);
                await algodclient.sendRawTransaction(RawSignedTxn).do();
                await writeTxAsync(payload['recordId'],txId);
                console.log(notemsg);
                //await waitForConfirmation(algodclient,txId,2);
                


            }else{
                //not opted it ot the asa at tiem of distibution, send a 0algo tx with a note ^^ rub it in
                //console.log("not opted in to asa "+assetID);

                let notOptedMsg = "Victory!\n"+selectedbot['MessageBody']+"\n\n"+boostmsg+"\nTotal rewards: "+amountToSend/selectedbot['Decimals']+" "+selectedbot['AsaName']+"!\n\n"+selectedbot['CTA']+noOpMsg+selectedbot['MessageEnd'];
                var note = enc.encode(notOptedMsg);
                let params = await algodclient.getTransactionParams().do();
                let txn = algosdk.makePaymentTxnWithSuggestedParams(senderAddress,receiverAddress, 0, undefined,note, params);
                let RawSignedTxn = txn.signTxn(senderAccount.sk);
                let txId = txn.txID().toString(); //can either write txid to db, or simply wait and write once confirmed(although that will keep process running for about 7 more seconds)
                //console.log("Signed transaction with txID: %s", txId);
                await algodclient.sendRawTransaction(RawSignedTxn).do();
                await writeTxAsync(payload['recordId'],"not opted in");
                console.log(notOptedMsg);

            }

            //send the fee generated on planet to planet pool
            if(payPlanetFee){
                //planet sink should be opted in always
                console.log("paying planet fee");
                var note = enc.encode(planetFeeMsg);
                let params = await algodclient.getTransactionParams().do();
                let txn = algosdk.makeAssetTransferTxnWithSuggestedParams(senderAddress, planetWalletAddress, undefined, undefined, planetFeeToSend,  note, assetID, params);
                let RawSignedTxn = txn.signTxn(senderAccount.sk);
                let txId = txn.txID().toString();
                console.log("Signed transaction with txID: %s", txId);
                await algodclient.sendRawTransaction(RawSignedTxn).do();
                console.log(planetFeeMsg);
            }

        }

    }else{
        console.log("###ALERT   ###ALERT   ###ALERT   No such bot entry, invalid item!"+botid);
        return;
    }
    

  } catch (err) {
    console.error("An error occurred xx");
    console.log(message);
    throw err;
  }
}