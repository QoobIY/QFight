import React, { useState } from "react";
import Editor from "react-simple-code-editor";
import { highlight, languages } from "prismjs";
// import "prismjs/components/prism-clike";
import "prismjs/components/prism-javascript";
import "prismjs/components/prism-lua";
import "prism-themes/themes/prism-xonokai.css";
import { WsHandler } from "./ws-handler";

const initCode = `    
-- defines a factorial function
  native = CS.NativeFunctions()
  nativeStatic = CS.NativeFunctions
  native:MoveToCenter(42)
  female = nativeStatic.FindByName('Female')
  male = nativeStatic.FindByName('Male')
  print('male', male, female)
  nativeStatic.FollowTo(female, male)
`;

if (typeof (window as any).global === "undefined") {
  (window as any).global = window;
  console.log("globalize?");
}


export const Highliter = ({ conn }: { conn: WsHandler | undefined }) => {
  const [code, setCode] = useState(initCode);

  const handleSendCode = () => {
    conn?.sendMessage("run-script", code);
  }
  return (
    <div>
      <Editor
        value={code}
        onValueChange={(_code) => setCode(_code)}
        highlight={(code) => highlight(code, languages.lua, "lua")}
        padding={10}
        style={{
          fontFamily: '"Fira code", "Fira Mono", monospace',
          fontSize: 12,
          border: "1px solid black",
        }}
      />
      <button onClick={handleSendCode}>Send code</button>
    </div>
  );
};
