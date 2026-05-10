'use strict';

const ByPassWallets = [
  "WALLET1", /* ios review */
  "WALLET2", /* ios2 review */
  "WALLET3", /* dev */
];

const BlacklistedWallets = [];

const NODEURL = "https://mainnet-idx.algonode.cloud";

const request = require('sync-request');

var mongoose = require('mongoose'),
  Wallet = mongoose.model('Wallets');


exports.checkaddress = function(req, res) {

  var walletaddress = req.params.walletId;

  if (BlacklistedWallets.indexOf(walletaddress) > -1) {
    res.json({valid: false, note: "blacklisted"});
    res.end();
  } else if (ByPassWallets.indexOf(walletaddress) > -1) {
    res.json({valid: true, note: "bypass"});
    res.end();
  } else {
    var walletvalid = false;

    Wallet.findById(req.params.walletId, function(err, wallet) {

      if (err) {
        res.json(err);
      } else {
        if (wallet != null) {
          res.json({valid: true, note: "fetched from cached: " + wallet.updated_date});
        } else {
          var resp = request('GET', `${NODEURL}/v2/assets/791381678/balances?limit=1000&currency-greater-than=0`, {
            headers: {
              'user-agent': 'example-user-agent',
            },
          });

          var bodydata = JSON.parse(resp.getBody('utf8'));

          for (let wal = 0; wal < bodydata.balances.length; wal++) {
            var current = bodydata.balances[wal];
            if (current.address == walletaddress) {
              walletvalid = true;
              break;
            }
          }

          if (walletvalid) {
            res.json({valid: true, note: "fetched fresh"});
          } else {
            res.json({valid: false, note: "invalid"});
          }

          res.end();
        }
      }
    });
  }
};


exports.updatedb = function(req, res) {
  Wallet.find({}, function(err, wallet) {
    if (err) {
      res.send(err);
    } else {
      var resp = request('GET', `${NODEURL}/v2/assets/791381678/balances?limit=1000&currency-greater-than=0`, {
        headers: {
          'user-agent': 'example-user-agent',
        },
      });

      var bodydata = JSON.parse(resp.getBody('utf8'));

      for (let wal = 0; wal < bodydata.balances.length; wal++) {
        var alreadypresent = false;
        var current = bodydata.balances[wal];
        for (let w = 0; w < wallet.length; w++) {
          if (wallet[w]._id == current.address) {
            alreadypresent = true;
            break;
          }
        }
        if (!alreadypresent) {
          let addwalletnew = new Wallet({
            _id: current.address,
            name: current.address,
            status: true
          });
          addwalletnew.save(function(errx) {
            if (errx) {
              // duplicate key on re-sync — expected, not an error
            }
          });
        }
      }

      var purgeids = [];
      for (let w = 0; w < wallet.length; w++) {
        var purgeid = wallet[w]._id;
        var purge = true;
        for (let wal = 0; wal < bodydata.balances.length; wal++) {
          if (purgeid == bodydata.balances[wal].address) {
            purge = false;
            break;
          }
        }
        if (purge) {
          purgeids.push(purgeid);
        }
      }
      Wallet.deleteMany({_id: {$in: purgeids}});
    }

    res.json("done");
  });
};


exports.updatecron = function() {
  Wallet.find({}, function(err, wallet) {
    if (err) return;

    var resp = request('GET', `${NODEURL}/v2/assets/791381678/balances?limit=1000&currency-greater-than=0`, {
      headers: {
        'user-agent': 'example-user-agent',
      },
    });

    var bodydata = JSON.parse(resp.getBody('utf8'));

    for (let wal = 0; wal < bodydata.balances.length; wal++) {
      var alreadypresent = false;
      var current = bodydata.balances[wal];
      for (let w = 0; w < wallet.length; w++) {
        if (wallet[w]._id == current.address) {
          alreadypresent = true;
          break;
        }
      }
      if (!alreadypresent) {
        let addwalletnew = new Wallet({
          _id: current.address,
          name: current.address,
          status: true
        });
        addwalletnew.save(function(errx) {
          if (errx) {
            // duplicate key on re-sync — expected, not an error
          }
        });
      }
    }

    var purgeids = [];
    for (let w = 0; w < wallet.length; w++) {
      var purgeid = wallet[w]._id;
      var purge = true;
      for (let wal = 0; wal < bodydata.balances.length; wal++) {
        if (purgeid == bodydata.balances[wal].address) {
          purge = false;
          break;
        }
      }
      if (purge) {
        purgeids.push(purgeid);
      }
    }
    Wallet.deleteMany({_id: {$in: purgeids}});
  });
};
