using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


[LuaCallCSharp]
class NativeFunctions
{
    public void MoveToCenter(float ff)
    {
        Debug.Log("Moving to center... " + ff);
    }

    public static GameObject FindByName(string name)
    {
        Debug.Log("Get by name " + name);
        return GameObject.Find(name);
    }

    public static void FollowTo(GameObject source, GameObject target)
    {
        Debug.Log("Follow to " + target);
        if (source.tag.Equals("Unit") && target.tag.Equals("Unit") && source != target)
        {
            Debug.Log("Follow to Unit " + target);
            source.GetComponent<QFight.Unit>().SetTargetUnit(target);
        }
    }

    public static void MoveTo(GameObject source, Vector3 target)
    {
        if (source.tag.Equals("Unit"))
        {
            source.GetComponent<QFight.Unit>().SetTargetPoint(target);
        }
    }
}



[LuaCallCSharp]
public class NativeQ : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        // NativeFunctions native = new NativeFunctions();

        // ExecuteCode("CS.UnityEngine.Debug.Log('hello lua')");
        // ExecuteCode("CS.UnityEngine.Debug.Log('hello xxx')");
        // ExecuteCode(@"
        //     native = CS.NativeFunctions()
        //     nativeStatic = CS.NativeFunctions
        //     native:MoveToCenter(42)

        //     female = nativeStatic.FindByName('Female')
        //     male = nativeStatic.FindByName('Male')

        //     print('male', male, female)
        //     nativeStatic.FollowTo(female, male)
        // ");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExecuteCode(string code)
    {
        LuaEnv luaenv = new LuaEnv();
        luaenv.DoString(code);
        luaenv.Dispose();
    }
}
