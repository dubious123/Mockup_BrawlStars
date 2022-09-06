using UnityEngine;
using TMPro;
using ServerCore.Packets;
using static Enums;
using UnityEngine.UI;

public class Scene_Login : BaseScene
{
	#region Serialize Fields
	[SerializeField] TMP_InputField _input_Id;
	[SerializeField] TMP_InputField _input_Pw;
	[SerializeField] TMP_Text _warning;
	[SerializeField] UITool_Fade _fadeTool;
	[SerializeField] Button _btn_Login;
	[SerializeField] string _warningText_IdEmpty;
	[SerializeField] string _warningText_PwEmpty;
	[SerializeField] string _warningText_LoginFailed;
	#endregion
	C_Login _packet;

	public override void Init(object param)
	{
		_packet = new C_Login();
	}
	public void Login()
	{
		if (_input_Id.text.Length == 0)
		{
			_warning.text = _warningText_IdEmpty;
			_fadeTool.StartFade();
			return;
		}
		if (_input_Pw.text.Length == 0)
		{
			_warning.text = _warningText_PwEmpty;
			_fadeTool.StartFade();
			return;
		}
		_packet.loginId = _input_Id.text;
		_packet.loginPw = _input_Pw.text;
		_btn_Login.enabled = false;
		Network.RegisterSend(_packet);
	}
	public void OnLoginFailed()
	{
		_warning.text = _warningText_LoginFailed;
		_fadeTool.StartFade();
		_btn_Login.enabled = true;
	}
	public void OnLoginSuccess(S_Login res)
	{
		User.Init(res);
		Scene.MoveTo(Enums.SceneType.Lobby, CharacterType.Dog);
	}
}
