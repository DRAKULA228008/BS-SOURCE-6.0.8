using UnityEngine;
using XLua;

public class SignatureLoaderTest : MonoBehaviour
{
	public static string PUBLIC_KEY = "BgIAAACkAABSU0ExAAQAAAEAAQBVDDC5QJ+0uSCJA+EysIC9JBzIsd6wcXa+FuTGXcsJuwyUkabwIiT2+QEjP454RwfSQP8s4VZE1m4npeVD2aDnY4W6ZNJe+V+d9Drt9b+9fc/jushj/5vlEksGBIIC/plU4ZaR6/nDdMIs/JLvhN8lDQthwIYnSLVlPmY1Wgyatw==";

	private void Start()
	{
		LuaEnv luaEnv = new LuaEnv();
		luaEnv.AddLoader(new SignatureLoader(PUBLIC_KEY, delegate(ref string filepath)
		{
			filepath = filepath.Replace('.', '/') + ".lua";
			TextAsset textAsset = (TextAsset)Resources.Load(filepath);
			return (textAsset != null) ? textAsset.bytes : null;
		}));
		luaEnv.DoString("\r\n            require 'signatured1'\r\n            require 'signatured2'\r\n        ");
		luaEnv.Dispose();
	}

	private void Update()
	{
	}
}
