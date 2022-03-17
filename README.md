# Introduction

This PoC illustrates errors in serialization with Dapr pub/sub and 64bit values.

## Docker setup

The sample is self contained with Docker and can be run locally with:

```sh
docker-compose up --build
```

## Upload dapr image from local build

```bash
export DAPR_REGISTRY=docker.io/shubham1172
export DAPR_TAG=dev
make build-linux
make docker-build
make docker-push
```

## Code walkthrough

The setup is with a publisher and a subscriber, named `owner` and `replicaone` respectively. On startup, `replicaone` invokes an Actor method in `owner` that publishes 5 records with the current time. The events are subscribed to by `replicaone`.

## Original output

When running the code in Docker the trailing output should look similar to:

```
replicaone_1       | Requesting data from proxy: 03e166ab-3e75-4d4d-943c-2ce767611dd0
owner_1            | Publishing record 1/5: {"int32":1778635775,"uint32":1778635775,"int64":637715205845980876,"uint64":637715205845980876,"base64long":"zJZQ2Jme2Qg=","base64int":"/9MDag=="}
replicaone_1       | Got event            : {"int32":1778635775,"uint32":1778635775,"int64":637715205845980900,"uint64":637715205845980900,"base64long":"zJZQ2Jme2Qg=","base64int":"/9MDag=="}
owner_1            | Publishing record 2/5: {"int32":1787383049,"uint32":1787383049,"int64":637715205854728150,"uint64":637715205854728150,"base64long":"1g/W2Jme2Qg=","base64int":"CU2Jag=="}
replicaone_1       | Got event            : {"int32":1787383049,"uint32":1787383049,"int64":637715205854728200,"uint64":637715205854728200,"base64long":"1g/W2Jme2Qg=","base64int":"CU2Jag=="}
owner_1            | Publishing record 3/5: {"int32":1789342999,"uint32":1789342999,"int64":637715205856688100,"uint64":637715205856688100,"base64long":"5Pfz2Jme2Qg=","base64int":"FzWnag=="}
replicaone_1       | Got event            : {"int32":1789342999,"uint32":1789342999,"int64":637715205856688100,"uint64":637715205856688100,"base64long":"5Pfz2Jme2Qg=","base64int":"FzWnag=="}
owner_1            | Publishing record 4/5: {"int32":1792380594,"uint32":1792380594,"int64":637715205859725695,"uint64":637715205859725695,"base64long":"f1Ei2Zme2Qg=","base64int":"so7Vag=="}
replicaone_1       | Got event            : {"int32":1792380594,"uint32":1792380594,"int64":637715205859725700,"uint64":637715205859725700,"base64long":"f1Ei2Zme2Qg=","base64int":"so7Vag=="}
owner_1            | Publishing record 5/5: {"int32":1796901791,"uint32":1796901791,"int64":637715205864246892,"uint64":637715205864246892,"base64long":"bE5n2Zme2Qg=","base64int":"n4saaw=="}
replicaone_1       | Got event            : {"int32":1796901791,"uint32":1796901791,"int64":637715205864246900,"uint64":637715205864246900,"base64long":"bE5n2Zme2Qg=","base64int":"n4saaw=="}
```

Note here that the `int64` and `uint64` values are "truncated" when received. For instance, the last row has the value `637715205864246892` but the recieved value is `637715205864246900`, showing a difference in the last two decimals. In all cases the last two decimals are set to zero. The binary value transmitted with base64 encoding is not altered, but shows that the sent and received values are _supposed_ to be the same.

## After migrating from json-iterator to encoding/json

```
replicaone_1       | Requesting data from proxy: 1ffca099-df2d-41b6-8676-2a08e2af5756
owner_1            | Publishing record 1/5: {"int32":545706050,"uint32":545706050,"int64":637830114315518474,"uint64":637830114315518474,"base64long":"CsLSDhwH2gg=","base64int":"QtCGIA=="}
replicaone_1       | Got event            : {"int32":545706050,"uint32":545706050,"int64":637830114315518500,"uint64":637830114315518500,"base64long":"CsLSDhwH2gg=","base64int":"QtCGIA=="}
owner_1            | Publishing record 2/5: {"int32":558100276,"uint32":558100276,"int64":637830114327912700,"uint64":637830114327912700,"base64long":"/OCPDxwH2gg=","base64int":"NO9DIQ=="}
replicaone_1       | Got event            : {"int32":558100276,"uint32":558100276,"int64":637830114327912700,"uint64":637830114327912700,"base64long":"/OCPDxwH2gg=","base64int":"NO9DIQ=="}
owner_1            | Publishing record 3/5: {"int32":561174761,"uint32":561174761,"int64":637830114330987185,"uint64":637830114330987185,"base64long":"scq\u002BDxwH2gg=","base64int":"6dhyIQ=="}
replicaone_1       | Got event            : {"int32":561174761,"uint32":561174761,"int64":637830114330987100,"uint64":637830114330987100,"base64long":"scq\u002BDxwH2gg=","base64int":"6dhyIQ=="}
owner_1            | Publishing record 4/5: {"int32":567815820,"uint32":567815820,"int64":637830114337628244,"uint64":637830114337628244,"base64long":"VCAkEBwH2gg=","base64int":"jC7YIQ=="}
replicaone_1       | Got event            : {"int32":567815820,"uint32":567815820,"int64":637830114337628300,"uint64":637830114337628300,"base64long":"VCAkEBwH2gg=","base64int":"jC7YIQ=="}
owner_1            | Publishing record 5/5: {"int32":575538210,"uint32":575538210,"int64":637830114345350634,"uint64":637830114345350634,"base64long":"6vWZEBwH2gg=","base64int":"IgROIg=="}
replicaone_1       | Got event            : {"int32":575538210,"uint32":575538210,"int64":637830114345350700,"uint64":637830114345350700,"base64long":"6vWZEBwH2gg=","base64int":"IgROIg=="}
```

## After components-contrib fix

```
replicaone-1       | Requesting data from proxy: 9fbef17c-f3a1-4c28-ad81-d734700a0930
owner-1            | Publishing record 1/5: {"int32":1943551969,"uint32":1943551969,"int64":637831348368977771,"uint64":637831348368977771,"base64long":"ay8kYjsI2gg=","base64int":"4T/Ycw=="}
replicaone-dapr-1  | time="2022-03-17T17:27:17.0045344Z" level=debug msg="Processing Redis message 1647538037004-0" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.contrib type=log ver=edge
replicaone-1       | Got event            : {"int32":1943551969,"uint32":1943551969,"int64":637831348368977771,"uint64":637831348368977771,"base64long":"ay8kYjsI2gg=","base64int":"4T/Ycw=="}
replicaone-dapr-1  | time="2022-03-17T17:27:17.0873581Z" level=debug msg="skipping status check due to error parsing result from pub/sub event 934d78b4-9252-4143-9c01-f7e9b10c9d51" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.runtime type=log ver=edge
issue-owner-1            | Publishing record 2/5: {"int32":1947637450,"uint32":1947637450,"int64":637831348373063252,"uint64":637831348373063252,"base64long":"VIZiYjsI2gg=","base64int":"ypYWdA=="}
issue-replicaone-dapr-1  | time="2022-03-17T17:27:17.3079611Z" level=debug msg="Processing Redis message 1647538037307-0" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.contrib type=log ver=edge
issue-replicaone-1       | Got event            : {"int32":1947637450,"uint32":1947637450,"int64":637831348373063252,"uint64":637831348373063252,"base64long":"VIZiYjsI2gg=","base64int":"ypYWdA=="}
issue-replicaone-dapr-1  | time="2022-03-17T17:27:17.3099919Z" level=debug msg="skipping status check due to error parsing result from pub/sub event dd746306-1680-4ecc-b1db-ee17bd8d0800" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.runtime type=log ver=edge
owner-1            | Publishing record 3/5: {"int32":1950839757,"uint32":1950839757,"int64":637831348376265559,"uint64":637831348376265559,"base64long":"V2OTYjsI2gg=","base64int":"zXNHdA=="}
replicaone-dapr-1  | time="2022-03-17T17:27:17.6284389Z" level=debug msg="Processing Redis message 1647538037628-0" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.contrib type=log ver=edge
replicaone-1       | Got event            : {"int32":1950839757,"uint32":1950839757,"int64":637831348376265559,"uint64":637831348376265559,"base64long":"V2OTYjsI2gg=","base64int":"zXNHdA=="}
replicaone-dapr-1  | time="2022-03-17T17:27:17.6302912Z" level=debug msg="skipping status check due to error parsing result from pub/sub event 6fd604b7-2d4e-4e1a-93f1-9a96bfdfe3fa" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.runtime type=log ver=edge
owner-1            | Publishing record 4/5: {"int32":1958501084,"uint32":1958501084,"int64":637831348383926886,"uint64":637831348383926886,"base64long":"ZkoIYzsI2gg=","base64int":"3Fq8dA=="}
replicaone-dapr-1  | time="2022-03-17T17:27:18.3960484Z" level=debug msg="Processing Redis message 1647538038395-0" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.contrib type=log ver=edge
replicaone-1       | Got event            : {"int32":1958501084,"uint32":1958501084,"int64":637831348383926886,"uint64":637831348383926886,"base64long":"ZkoIYzsI2gg=","base64int":"3Fq8dA=="}
replicaone-dapr-1  | time="2022-03-17T17:27:18.3989585Z" level=debug msg="skipping status check due to error parsing result from pub/sub event d7d5c216-df7a-423a-8c1a-d6b6776bde43" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.runtime type=log ver=edge
owner-1            | Publishing record 5/5: {"int32":1960827506,"uint32":1960827506,"int64":637831348386253308,"uint64":637831348386253308,"base64long":"/MkrYzsI2gg=","base64int":"ctrfdA=="}
replicaone-dapr-1  | time="2022-03-17T17:27:18.6266703Z" level=debug msg="Processing Redis message 1647538038626-0" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.contrib type=log ver=edge
replicaone-1       | Got event            : {"int32":1960827506,"uint32":1960827506,"int64":637831348386253308,"uint64":637831348386253308,"base64long":"/MkrYzsI2gg=","base64int":"ctrfdA=="}
replicaone-dapr-1  | time="2022-03-17T17:27:18.6281602Z" level=debug msg="skipping status check due to error parsing result from pub/sub event 33a9f3c6-3f46-4ae6-afd7-5d572f4bc464" app_id=replicaoneapp instance=72c717be8b45 scope=dapr.runtime type=log ver=edge
```