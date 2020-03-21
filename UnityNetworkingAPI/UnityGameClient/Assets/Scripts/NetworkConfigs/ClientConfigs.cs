using UnityEngine;

[CreateAssetMenu]
public class ClientConfigs : ScriptableObject
{
    [SerializeField] int bufferSize = 4096;
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 26950;
    [SerializeField] string userName = "username";
    public string Ip => ip;
    public int Port => port;
    public int BufferSize => bufferSize;

    public string UserName
    {
        get => userName;
        set => userName = value;
    }
}