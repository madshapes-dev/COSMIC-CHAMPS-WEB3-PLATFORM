import { DynamoDBClient } from "@aws-sdk/client-dynamodb";
import { DynamoDBDocumentClient, UpdateCommand } from "@aws-sdk/lib-dynamodb";
import { marshall, unmarshall } from "@aws-sdk/util-dynamodb";
import { SendMessageCommand, SQSClient } from "@aws-sdk/client-sqs";

const client = new DynamoDBClient({});
const docClient = DynamoDBDocumentClient.from(client);
const sqsclient = new SQSClient({});


const TABLE_NAME = "MatchReport";
//"MatcReportingTest1"

//enable to send all results to a logger queue
const logResults = true;

//for easier searching, list all bots
//order if bots listed is important, for logic later on...
const BOTSARRAY = [

    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE1", /*training dummy*/
    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE2", /*default stargazer*/
    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE3", /*default1 rising star*/
    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE4", /*default2 cosmic explorer*/
    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE5", /*default3 planet lord */
    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE6", /*default4 star destroyer */

    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE7", /* cosmic bot*/
    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE8", /*cosmic bot 1 */
    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEE9", /* cb2 */
    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE10", /* cb3 */
    "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE11", /* cb4 */

    /* algogems bot */
    /* vestige bot */
    /* cometa bot */
    /* algoleagues bot */
    /* robocoop bot */
    /* goanna bot */
    /* tiny bot */
    /* rxelms bot */
    /* marcus bot */
    /* liquihog */
    /* alphaarcade */
    /* folks finance */
    /* compx bot */

];


export const handler = async (event) => {
    console.log('## EVENT: ' + serialize(event));
    //we really only expect one insert at each given time, as batch size for stream is 1
    //only handle stuff is it's an INSERT obviously
    //we also handle filtering of the inserts only on stream level, but this is fallback for sake of sanity 
    //filters wont work when invoking test events!
    if(event.Records[0].eventName == "INSERT"){
         const record = unmarshall(event.Records[0].dynamodb.NewImage);
         
         if(!record.WinnerId){
             console.log("#####broken entry, return#####");
             docClient.destroy(); // no-op, no clue if needed
             client.destroy(); // destroys DynamoDBClient
             sqsclient.destroy();
             console.log("cleanup done");
             return;
         }
         
         //some checks
         //need to use ['S'] stuff to properly fetch dynamodb stuff...nvm use unmarshall
         if(record.Environment !== "Development" ){
        //if(record.Environment == "TestingB" ){

            //assign winner and loser player    
            var WinnerPlayer = record.Players[0];
            var LoserPlayer = record.Players[1];
            if(record.Players[0].Id !== record.WinnerId){
                //fix if we assumed order wrongly
                WinnerPlayer = record.Players[1];
                LoserPlayer = record.Players[0];
            }


            //send event to logger queue
            if(logResults){
              /////////////////
              //this is an extra event generated and pushed to a DIFFERENT SQS!
              ////////////////
              //some duplicate code here, fck it.... easier than refactoring it all...
              

              var pushItem = {};
                            
              pushItem['recordId'] = record.Id;

              pushItem['winnerId'] = WinnerPlayer.Id;   
              pushItem['winnerNickname'] = WinnerPlayer.Nickname;
              pushItem['winnerRating'] = WinnerPlayer.Rating;
              pushItem['winnerHUD'] = "EMPTY";
              if(WinnerPlayer.HUDSkin){
                  pushItem['winnerHUD'] = WinnerPlayer.HUDSkin;
              }

              //add wallet address to both players (bots have same wallet addr in both fields, players might not have both)
              if(WinnerPlayer.LinkedWalletId){
                pushItem['winnerWalletId'] = WinnerPlayer.LinkedWalletId;
              }else{
                  pushItem['winnerWalletId'] = WinnerPlayer.WalletId;
              }

              pushItem['loserId'] = LoserPlayer.Id;
              pushItem['loserNickname'] = LoserPlayer.Nickname;
              pushItem['loserRating'] = LoserPlayer.Rating;
              pushItem['loserHUD'] = "EMPTY";
              if(LoserPlayer.HUDSkin){
                  pushItem['loserHUD'] = LoserPlayer.HUDSkin;
              }

              //add wallet address to both players (bots have same wallet addr in both fields, players might not have both)
              if(LoserPlayer.LinkedWalletId){
                  pushItem['loserWalletId'] = LoserPlayer.LinkedWalletId;
              }else{
                  pushItem['loserWalletId'] = LoserPlayer.WalletId;
              }

              //small flag to help identify if there was a drawn game or not
              if(record.WinnerId == "DRAWN"){
                pushItem['Result'] = "DRAWN";
              }else{
                pushItem['Result'] = "OK";
              }

              console.log('## PAYLOAD - logger: '+serialize(pushItem));

              var sqscommand = new SendMessageCommand({
                  QueueUrl: 'https://sqs.eu-central-1.amazonaws.com/QUEUEID/LoggerQueue.fifo',
                  MessageDeduplicationId: pushItem.recordId,
                  MessageGroupId: "LoggerEvents",
                  MessageBody: JSON.stringify(pushItem),
              });
              
              var sqsresponse = await sqsclient.send(sqscommand);
              console.log('## SQS + logger RESPONSE: '+serialize(sqsresponse));
              //
            }


             if(record.WinnerId !== "DRAWN"){
                //console.log("not drawn");
                
                  
   
                if(BOTSARRAY.includes(record.WinnerId)){
                    console.log("bot won, do nothing");
                     var command = new UpdateCommand({
                      TableName: TABLE_NAME,
                      Key: {
                        Id: record.Id,
                      },
                      UpdateExpression: 'set ProcessingResult = :r',
                      ExpressionAttributeValues: {
                        ':r': 'VS BOT ('+WinnerPlayer.Nickname+') LOSS - no prize',
                      },
                      ReturnValues: "UPDATED_NEW",
                    });
                  
                  var response = await docClient.send(command);

                }else{
                    //console.log("player won, check prize eligibility");

                    //check if loser is a prize/bounty bot
                    //forst 6 bots are not prize bots
                    /////////////////////////////////////////////////FIRST 6 BOTS ARE NOT PRIZE BOTS//////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////FIRST 6 BOTS ARE NOT PRIZE BOTS//////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////FIRST 26BOTS ARE NOT PRIZE BOTS//////////////////////////////////////////////////////////

                    if(BOTSARRAY.includes(LoserPlayer.Id)){
                        //console.log("player "+WinnerPlayer.Nickname+" defeated a BOT ("+LoserPlayer.Nickname+")");

                        //0 thourgh 5 are regular bots, 
                        if(BOTSARRAY.indexOf(LoserPlayer.Id) > 5){
                            console.log("player "+WinnerPlayer.Nickname+" defeated a prize paying BOT ("+LoserPlayer.Nickname+") "+LoserPlayer.Id+" -> push prize to queue");
                            //console.log("a prize paying bot "+LoserPlayer.Id+" , nice - push the payment to queue!");
                            //console.log("indexof="+BOTSARRAY.indexOf(LoserPlayer.Id));

                            var pushItem = {};
                            
                            pushItem['recordId'] = record.Id;
                            pushItem['winnerId'] = WinnerPlayer.Id;
                            
                            
                            //if player has linkedwallet use that for prize payouts
                            if(WinnerPlayer.LinkedWalletId){
                                pushItem['winnerWalletId'] = WinnerPlayer.LinkedWalletId;
                            }else{
                                pushItem['winnerWalletId'] = WinnerPlayer.WalletId;
                            }
                            
                            pushItem['winnerNickname'] = WinnerPlayer.Nickname;
                            pushItem['winnerRating'] = WinnerPlayer.Rating;
                            pushItem['winnerHUD'] = "EMPTY";
                            if(WinnerPlayer.HUDSkin){
                                pushItem['winnerHUD'] = WinnerPlayer.HUDSkin;
                            }
                            pushItem['loserId'] = LoserPlayer.Id;
                            pushItem['loserWalletId'] = LoserPlayer.WalletId;
                            pushItem['loserNickname'] = LoserPlayer.Nickname;

                            console.log('## PAYLOAD: '+serialize(pushItem));
                            
                            //no need to write to db here, as we'll sort it when processed
                            //actually write to db, to know we've sent to queue(with 5min delay)
                            //so user has time to opt-in
                                 var command = new UpdateCommand({
                                  TableName: TABLE_NAME,
                                  Key: {
                                    Id: record.Id,
                                  },
                                  UpdateExpression: 'set ProcessingResult = :r',
                                  ExpressionAttributeValues: {
                                    ':r': '->',
                                  },
                                  ReturnValues: "UPDATED_NEW",
                                });
                              
                              var response = await docClient.send(command);
                              
                            
                            //prepare and send data to SQS


                            //make sure to add +'XYZ' to deduplicationId when testing, or change insert id
                            //else it wont propagate to sqs as it has deduplication enabled
                            var sqscommand = new SendMessageCommand({
                                QueueUrl: 'https://sqs.eu-central-1.amazonaws.com/QUEUEID/PrizeBotPayoutsQueue.fifo',
                                MessageDeduplicationId: pushItem.recordId,
                                MessageGroupId: "PrizeBotPayments",
                                MessageBody: JSON.stringify(pushItem),
                            });
                            
                            var sqsresponse = await sqsclient.send(sqscommand);
                            console.log('## SQS RESPONSE: '+serialize(sqsresponse));
                            

                        }else{
                            console.log("player "+WinnerPlayer.Nickname+" defeated a BOT ("+LoserPlayer.Nickname+") - no prize");
                            //console.log("just a regular bot, no prize")
                            
                            //console.log("id:"+record.Id);
                            var command = new UpdateCommand({
                                TableName: TABLE_NAME,
                                Key: {
                                  Id: record.Id,
                                },
                                UpdateExpression: 'set ProcessingResult = :r',
                                ExpressionAttributeValues: {
                                  ':r': 'VS BOT ('+LoserPlayer.Nickname+') win - no prize',
                                },
                                ReturnValues: "UPDATED_NEW",
                              });
                            
                            var response = await docClient.send(command);
                        }


                    }else{
                        console.log("player "+WinnerPlayer.Nickname+" defeated another player ("+LoserPlayer.Nickname+") - no prize"); 
                        
                        //console.log("id:"+record.Id);
                        var command = new UpdateCommand({
                            TableName: TABLE_NAME,
                            Key: {
                              Id: record.Id,
                            },
                            UpdateExpression: 'set ProcessingResult = :r',
                            ExpressionAttributeValues: {
                              ':r': 'VS PLAYER ('+LoserPlayer.Nickname+') win - no prize',
                            },
                            ReturnValues: "UPDATED_NEW",
                          });
                        
                        var response = await docClient.send(command);
                    }
        
                }

              
             }else{
                console.log("drawn match - no prize");  
                
                //console.log("id:"+record.Id);
                 var command = new UpdateCommand({
                    TableName: TABLE_NAME,
                    Key: {
                      Id: record.Id,
                    },
                    UpdateExpression: 'set ProcessingResult = :r',
                    ExpressionAttributeValues: {
                      ':r': 'DRAW - no prize',
                    },
                    ReturnValues: "UPDATED_NEW",
                  });
                
                var response = await docClient.send(command);
                
             }
         }

    }//end insert clause
    //can add else statement to handle non-inserts if needed at later date
    
    
    //clean up dynamodb stuff
    docClient.destroy(); // no-op, no clue if needed
    client.destroy(); // destroys DynamoDBClient
    sqsclient.destroy();
    console.log("cleanup done");

};
  

var serialize = function(object) {
  return JSON.stringify(object, null, 2)
}