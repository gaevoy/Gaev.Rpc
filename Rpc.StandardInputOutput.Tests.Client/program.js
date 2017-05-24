var stdin = require('readline').createInterface({ input: process.stdin });
var stdout = process.stdout;
var Ping = "Rpc.StandardInputOutput.Tests.Client.Ping, Rpc.StandardInputOutput.Tests.Client";
var Pong = "Rpc.StandardInputOutput.Tests.Client.Pong, Rpc.StandardInputOutput.Tests.Client";

stdin.on('line', function (json) {
    var rpc = JSON.parse(json);
    rpc.Payload = receive(rpc.Payload);
    stdout.write(JSON.stringify(rpc) + "\n");
});

function receive(msg) {
    if (msg.$type === Ping) {
        return {
            $type: Pong,
            Header: "Hello from NodeJS",
            Data: msg.Data
        };
    }
}