using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static Enums;

public class Scene_Login : BaseScene
{
	#region Serialize Fields
	[SerializeField] private TMP_InputField _input_Id;
	[SerializeField] private TMP_InputField _input_Pw;
	[SerializeField] private TMP_Text _warning;
	[SerializeField] private UITool_Fade _fadeTool;
	[SerializeField] private Button _btn_Login;
	[SerializeField] private string _warningText_IdEmpty;
	[SerializeField] private string _warningText_PwEmpty;
	[SerializeField] private string _warningText_LoginFailed;
	#endregion
	private C_Login _packet;

	public override void Init(object param)
	{
		Scenetype = SceneType.Login;
		_packet = new C_Login();
		IsReady = true;
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
		Scene.MoveTo(Enums.SceneType.Lobby, CharacterType.Knight);
	}
}
