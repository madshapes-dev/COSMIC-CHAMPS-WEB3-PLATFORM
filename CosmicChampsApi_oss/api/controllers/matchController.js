'use strict';

var mongoose = require('mongoose'),
Match = mongoose.model('Matches');



exports.matchstart = function(req, res) {
    var nmatch = new Match({
        _id:req.params.matchId,
        playera:req.params.pa,
        playerb:req.params.pb
    });

    nmatch.save(function(err, matchh) {
      if (err)
        res.send(err);
      res.json(matchh);
    });
  };


  exports.matchend = function(req, res) {

    var update = 
    {
        winner:req.params.w,
        end_date:Date.now()
    };
    var filter = {_id: req.params.matchId};

    Match.findOneAndUpdate(filter, update, {new: false}, function(err, mtch) {
        if (err)
          res.send(err);
        res.json(mtch);
      });
  }; 






exports.getMatchdata = function(req,res){
 var lbconstraints = req.params.from;
 //console.log(lbconstraints);
  if(lbconstraints == "all"){
    //override leaderboard to provide data for "alltime"
    lbconstraints = "2022-12-05";
  }
  lbconstraints += "T00:00"; //just append hours/minutes if sprints etc start difernetly so we know



  Match.aggregate([
    {
      '$match': {
        'winner': {
          '$nin': ['NONE',null]
        }, 
        'start_date': {
          '$gte': new Date(lbconstraints)
        }
      }
    }, {
      '$project': {
        'playera': 1, 
        'playerb': 1, 
        'winner': 1, 
        '_id': 0, 
        'duration': {
          '$subtract': [
            '$end_date', '$start_date'
          ]
        }
      }
    }
  ],function(errx, docsx) {

    var buildresponse = [];
    if(errx){
      console.log("error fetching lb for:"+lbconstraints);
    }else{
      //console.log(docsx);
      //make it prettier
      //var len = ((await agg).length);
      for (var doc of docsx) {

        if(!buildresponse[doc.playera]){
        //create new object
          buildresponse[doc.playera] = {'id':doc.playera,'gamesplayed':0,'totalduration':0,'gameswon':0,'drawn':0};

        }
        if(!buildresponse[doc.playerb]){
          buildresponse[doc.playerb] = {'id':doc.playerb,'gamesplayed':0,'totalduration':0,'gameswon':0,'drawn':0};
        }
        
        buildresponse[doc.playera].gamesplayed += 1;
        buildresponse[doc.playera].totalduration += doc.duration;
        buildresponse[doc.playerb].gamesplayed += 1;
        buildresponse[doc.playerb].totalduration += doc.duration;

        if(doc.playera == doc.winner){
          buildresponse[doc.playera].gameswon += 1;
        }
        else if(doc.playerb == doc.winner){
          buildresponse[doc.playerb].gameswon += 1;
        }else if(doc.winner == "DRAWN"){
          buildresponse[doc.playera].drawn += 1;
          buildresponse[doc.playerb].drawn += 1;
        }
       
      }
    }

    var adjsutedresp = [];
    for (const [key, value] of Object.entries(buildresponse)) {
      //if(value.gamesplayed >= 5){
        adjsutedresp.push(value);
      //}
    }

    //console.log(adjsutedresp); 

    res.json(adjsutedresp);
  });

};


exports.list_all = function(_req, res) {
    Match.find({}, function(err, mtch) {
      if (err)
        res.send(err);
      res.json(mtch);
    });
  };



  