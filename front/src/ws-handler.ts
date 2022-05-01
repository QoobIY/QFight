import * as PIXI from "pixi.js";

type StateChanger = (d: string) => void;

interface Coord {
  name: string;
  x: number;
  z: number;
  y: number;
}

export class WsHandler {
  ws: WebSocket;
  stateChanger: StateChanger;
  app: PIXI.Application;
  container: PIXI.Container;

  constructor(setState: StateChanger) {
    console.log("constructor");
    this.app = new PIXI.Application();
    this.container = new PIXI.Container();

    this.app.stage.addChild(this.container);
    document.querySelector(".canvas")?.appendChild(this.app.view);

    this.stateChanger = setState;
    this.ws = new WebSocket("ws://localhost:3000/name");
    this.ws.onopen = () => {
      this.ws.addEventListener("message", this.onMessage.bind(this));
      this.ws.addEventListener("close", this.onClose.bind(this));
      this.stateChanger("Successfull");
    };
  }

  onMessage(ev: MessageEvent) {
    const data = JSON.parse(ev.data);
    console.log("Message received", data);
    if (data.type === "update-coords") {
      console.log("this", this);
      this.redrawCoords(data.data);
    }
  }

  onClose() {
    console.log("Close server");
    this.stateChanger("Closed");
  }

  redrawCoords(coords: Coord[]) {
    for (var i = this.container.children.length - 1; i >= 0; i--) {
      this.container.removeChild(this.container.children[i]);
    }

    const { width, height } = this.app.view;

    const graphics = new PIXI.Graphics();

    coords.forEach((coord) => {
      const transformedX = (coord.x / 30) * width;
      const transformedY = height - (coord.z / 30) * height;
      graphics.beginFill(0xde3249, 1);
      graphics.drawCircle(transformedX, transformedY, 10);
      graphics.endFill();
      const text = new PIXI.Text(coord.name, {
        fontFamily: "Arial",
        fontSize: 24,
        fill: 0xff1010,
        align: "center",
      });
      text.x = transformedX;
      text.y = transformedY;
      this.container.addChild(text);
    });
    this.container.addChild(graphics);
  }

  sendMessage<T>(type: string, data: T) {
    this.ws.send(JSON.stringify({
      type: type,
      data: data,
    }));
  }
}
