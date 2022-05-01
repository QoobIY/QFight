import { useEffect, useState } from "react";

import { Highliter } from "./Highliter";
import { WsHandler } from "./ws-handler";

interface IWSProps {
  conn: WsHandler | undefined;
  setConn(conn: WsHandler): void;
}

const WS: React.FC<IWSProps> = ({ conn, setConn }) => {
  const [state, setState] = useState<string>("Loading...");

  console.log("rerender");

  useEffect(() => {
    console.log("effect");
    return setConn(new WsHandler(setState));
  }, []);

  return <div>Websocket status {state}</div>;
};

const Counter = () => {
  useEffect(() => {
    console.log("effect counter");
    // return setConn(new WsHandler(setState));
  }, []);

  return <div>Counter</div>;
};

const App = () => {
  const [conn, setConn] = useState<WsHandler>();
  console.log("rerender app");

  useEffect(() => {
    console.log("effect app");
    // return setConn(new WsHandler(setState));
  }, []);

  return (
    <div className="container">
      <div className="canvas"></div>
      <div className="menu">
        <WS conn={conn} setConn={setConn} />
        <br />
        Lua Code Here:
        <div>
          <Highliter conn={conn} />
        </div>
      </div>
      <Counter />
    </div>
  );
};

export default App;
