
public interface IClientComponentSystem
{
	public void OnNetFrameUpdate();
	public void Interpretate(float t);
	public void Reset();
	public void Clear();
	public void OnRoundStart();
	public void OnRoundEnd();
}
