import algosdk from 'algosdk';


import { DynamoDBDocumentClient, PutCommand, GetCommand } from "@aws-sdk/lib-dynamodb";
import { DynamoDBClient } from "@aws-sdk/client-dynamodb";


const ddbClient = new DynamoDBClient({ region: "eu-west-1" });
const ddbDocClient = DynamoDBDocumentClient.from(ddbClient);


const TABLE_NAME = "user-walletid-autoassigned";


/**
*expects request in form of:
*   
{
    operation -> [getWallet, requestNewWallet, echo, read]

    "operation":"requestNewWallet",
    "payload":{
        "auth":"someauthkey",
        "userId":"someid"
    }
}


 * 
 */


///uncomment when deploy to aws
export const handler = async (event, context) => {
///


//if using lambdas "function url" endpoint instead of going through API gateway you need to use event.body instead of event iteslf
//will leave the event based code commented for future revisit and migration to api gateway

//uncomment for localtesting
/*(async () => {*/
//end repl

///////////////////////////////////////////////////////
/////////////func body//////////////////////////////////


    ///event based approach
    ///const body = event;

    //bypassing events from api gateway, so fetch body directly
     const body = JSON.parse(event.body);
     
     //no changes required below this line

     const operation = body.operation;
     
  
     if (operation == 'echo'){
          return(body.payload);
     }
     
     else if(operation == 'getWallet'){
        //will create new if not exists or simply return if already present
        if(body.payload.auth){
            let authdev = process.env.AuthDev;
            let authmaster = process.env.AuthMaster;
            
            if(authdev == body.payload.auth){
                //auth ok
            }else if(authmaster == body.payload.auth){
                console.log("----------MASTER_AUTH used------------");
            }else{
               return ({'error':'invalid auth provided'});
            }
        }else{
            return ({'error':'no auth provided'});
            //return ('no auth provided');
        }

        try{
            let acc = algosdk.generateAccount();
            //console.log("addr: "+acc.addr);
            let isodate = new Date().toISOString();
            
            const pcommand = new PutCommand({
                TableName: TABLE_NAME,
                Item: {
                    PlayerId:body.payload.userId,
                    WalletId: acc.addr,
                    WalletHash: acc.sk,
                    CreatedAt:isodate
                },
                ConditionExpression: 'attribute_not_exists(PlayerId)'
            });
            
            const data = await ddbDocClient.send(pcommand);
            //console.log(data);
            return ({'success':acc.addr});
            //return(acc.addr);
        }catch(error){
            //handle for when asking to insert already existing item
            /*console.log("conditionExpression failed");
            return ({'error':'already existing playerid'});
            //return ('already existing playerid');*/
            
            //read item and return it
            const rcommand = new GetCommand({
                TableName: TABLE_NAME,
                Key: {
                  PlayerId: body.payload.userId,
                },
            });
            
            const response = await ddbDocClient.send(rcommand);
            
            if(response.Item != null){
                //return response.Item.WalletId;
                return ({'success':response.Item.WalletId});
            }else{
                //return ('invalid playerid');
                return ({'error':'error reading player wallet...'});
            }
        }

     }
     
     
     
     else if(operation == 'requestNewWallet'){
         
         if(body.payload.auth){
             let authdev = process.env.AuthDev;
             let authmaster = process.env.AuthMaster;
             
             if(authdev == body.payload.auth){
                 //auth ok
             }else if(authmaster == body.payload.auth){
                 console.log("----------MASTER_AUTH used------------");
             }else{
                return ({'error':'invalid auth provided'});
             }
         }else{
             return ({'error':'no auth provided'});
             //return ('no auth provided');
         }
        
        try{
            let acc = algosdk.generateAccount();
            let isodate = new Date().toISOString();
            
            const pcommand = new PutCommand({
                TableName: TABLE_NAME,
                Item: {
                    PlayerId:body.payload.userId,
                    WalletId: acc.addr,
                    WalletHash: acc.sk,
                    CreatedAt:isodate
                },
                ConditionExpression: 'attribute_not_exists(PlayerId)'
            });
            
            const data = await ddbDocClient.send(pcommand);
            //console.log(data);
            return ({'success':acc.addr});
            //return(acc.addr);
        }catch(error){
            //handle for when asking to insert already existing item
            console.log("conditionExpression failed");
            return ({'error':'already existing playerid'});
            //return ('already existing playerid');
        }
        
     }else if(operation == 'read'){
         let authdev = process.env.AuthDev;
         let authmaster = process.env.AuthMaster;
         
         if(authdev == body.payload.auth){
             //auth ok
         }else if(authmaster == body.payload.auth){
             console.log("----------MASTER_AUTH used------------");
         }else{
            //return ('invalid auth provided');
            return ({'error':'invalid auth provided'});
         }
         
          const rcommand = new GetCommand({
                TableName: TABLE_NAME,
                Key: {
                  PlayerId: body.payload.userId,
                },
            });
            
            const response = await ddbDocClient.send(rcommand);
            
            if(response.Item != null){
                //return response.Item.WalletId;
                return ({'success':response.Item.WalletId});
            }else{
                //return ('invalid playerid');
                return ({'error':'invalid playerid'});
            }

            
     }
     
     else{
          return ('Unknown operation');
     }
     
   

          
////////////////////////end func body////////////////////////////////
//////////////////////////////////////////////////////////////////


///replace when in aws    
};
///


//repl fpr local testing
/*})().catch(e => {
   console.log(e);
   console.trace();
});*/
//end repl