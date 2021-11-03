# Introduction

This PoC illustrates errors in serialization with Dapr pub/sub and 64bit values.

## Docker setup

The sample is self contained with Docker and can be run locally with:

```sh
docker-compose up --build
```

## Code walkthrough

The setup is with a publisher and a subscriber, named `owner` and `replicaone` respectively. On startup, `replicaone` invokes an Actor method in `owner` that publishes 5 records with the current time. The events are subscribed to by `replicaone`.

## Sample output

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