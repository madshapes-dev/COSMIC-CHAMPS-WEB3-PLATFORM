var CronJob = require("node-cron");

var express = require('express'),
  app = express(),
  port = process.env.PORT || 3000,
  mongoose = require('mongoose'),
  Wallet = require('./api/models/ccapiModel'),
  Match = require('./api/models/matchModel'),
  Inventory = require('./api/models/inventoryModel'),
  NftSnapshots = require('./api/models/nftsnapshotModel'),
  bodyParser = require('body-parser');

mongoose.Promise = global.Promise;
mongoose.connect(process.env.MONGO_URI || 'mongodb://localhost/ccapidb');

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

var routes = require('./api/routes/ccapiRoutes');
routes(app);

var inventoryContr = require('./api/controllers/inventoryController');

// cron: uncommons every 57 seconds
const scheduledJobUpdateNftHolders = CronJob.schedule("*/57 * * * * *", () => {
  inventoryContr.updatedbcron();
});
scheduledJobUpdateNftHolders.start();

// cron: red chroma / commons / space rocks every 3 minutes
const scheduledJobUpdateRedChroma = CronJob.schedule('*/3 * * * *', () => {
  inventoryContr.updatedbcron_ps();
});
scheduledJobUpdateRedChroma.start();

// cron: rares every 7 minutes
const raresschedule = CronJob.schedule('*/7 * * * *', () => {
  inventoryContr.updateRaresCronPS();
});
raresschedule.start();

// cron: epics every 4 minutes
const epicschedule = CronJob.schedule("*/4 * * * *", () => {
  inventoryContr.updateEpicCronPS();
});
epicschedule.start();

// cron: legendaries every 13 minutes
const leggieschedule = CronJob.schedule("*/13 * * * *", () => {
  inventoryContr.updateLeggCronPS();
});
leggieschedule.start();

app.listen(port);
console.log('CC RESTful API server started on: ' + port);
