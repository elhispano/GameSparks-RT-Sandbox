using System.Collections.Generic;
using GameSparks.Api.Messages;

public class RTSession
{
    private readonly string m_acccessToken;
    private readonly string m_hostUrl;

    private readonly string m_matchId;

    private readonly List<RtPlayer> m_playerList = new List<RtPlayer>();

    private readonly int m_portId;

    /// <summary>
    /// Creates a new RTSession object which is held until a new RT session is created
    /// </summary>
    /// <param name="message">Message.</param>
    public RTSession(MatchFoundMessage message)
    {
        m_portId = (int) message.Port;
        m_hostUrl = message.Host;
        m_acccessToken = message.AccessToken;
        m_matchId = message.MatchId;
        // we loop through each participant and get their peerId and display name //
        foreach (var p in message.Participants) m_playerList.Add(new RtPlayer(p.DisplayName, p.Id, (int) p.PeerId));
    }

    public string GetHostUrl()
    {
        return m_hostUrl;
    }

    public string GetAccessToken()
    {
        return m_acccessToken;
    }

    public int GetPortId()
    {
        return m_portId;
    }

    public string GetMatchId()
    {
        return m_matchId;
    }

    public List<RtPlayer> GetPlayerList()
    {
        return m_playerList;
    }

    public class RtPlayer
    {
        public string DisplayName;
        public string Id;
        public bool IsOnline;
        public int PeerId;

        public RtPlayer(string displayName, string id, int peerId)
        {
            DisplayName = displayName;
            Id = id;
            PeerId = peerId;
        }
    }
}