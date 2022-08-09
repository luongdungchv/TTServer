import { Room, matchMaker } from "colyseus";

export class MatchMakerRoom extends Room {
    private pending;
    private roomList;
    constructor() {
        super();
        this.pending = null;
        this.roomList = [];
    }
    // this room supports only 4 clients connected
    maxClients = 4;

    onCreate(options) {
        console.log("Match created!", options);

        this.onMessage("message", (client, message) => {
            console.log(
                "ChatRoom received message from",
                client.sessionId,
                ":",
                message
            );
            this.broadcast("messages", `(${client.sessionId}) ${message}`);
            //client.send()
        });
    }

    onJoin(client) {
        if (this.pending == null) {
            this.pending = client;
            return;
        }
        let p1 = this.pending;
        let p2 = client;

        this.createRoom(p1, p2);
        this.pending = null;

        this.broadcast("messages", `${client.sessionId} joined.`);
    }

    onLeave(client) {
        this.broadcast("messages", `${client.sessionId} left.`);
    }

    onDispose() {
        console.log("Dispose Match Maker");
    }
    async createRoom(p1, p2) {
        var reserveP1 = await matchMaker.create("duel");
        console.log(JSON.stringify(reserveP1));
        var reserveP2 = await matchMaker.joinById(reserveP1.room.roomId);
        p1.send("pair", JSON.stringify(reserveP1));
        p2.send("pair", JSON.stringify(reserveP2));
        //this.send()
    }
}
