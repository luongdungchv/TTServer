import { Room } from "colyseus";

export class DuelRoom extends Room {
    // this room supports only 4 clients connected
    maxClients = 2;

    onCreate(options) {
        console.log("Duel created!", options);

        console.log(this.clients.length);

        this.onMessage("message", (client, message) => {
            console.log(
                "ChatRoom received message from",
                client.sessionId,
                ":",
                message
            );
            if (this.clients.length == 2) {
                let p1 = this.clients[0];
                let p2 = this.clients[1];
                let other = client.sessionId === p1.sessionId ? p2 : p1;
                client.send("message", "pr");
                other.send("message", message);
            }
        });
    }

    onJoin(client) {
        console.log(client.sessionId);
        let p1 = this.clients[0];
        let p2 = this.clients[1];
        console.log(this.clients.length);
        if (this.clients.length == 2) {
            p1.send("message", `${p1.sessionId} 0 0`);
            p2.send("message", `${p2.sessionId} 0 1`);
        }
        this.broadcast("note", `${client.sessionId} joined.`);
    }

    onLeave(client) {
        this.broadcast("note", `${client.sessionId} left.`);
    }

    onDispose() {
        console.log("Dispose Duel");
    }
}
