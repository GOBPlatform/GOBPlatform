using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

/// <summary>
/// common
/// </summary>
namespace GOBCommon
{
    public class LimiteWriter
    {
        private UInt64 count;
        private UInt64 max;
        private StreamWriter writer;

        public ulong Count { get => count; set => count = value; }
        public ulong Max { get => max; set => max = value; }
        public StreamWriter Writer { get => writer; set => writer = value; }
    }

    public class ZeroCopySource
    {
        private byte[] s;
        private UInt64 off;

        public byte[] S { get => s; set => s = value; }
        public ulong Off { get => off; set => off = value; }
    }
}

/// <summary>
/// config
/// </summary>
namespace GOBConfig
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class GenesisConfig
    {
        private string[] seedList;
        private string consensusType;
        private VBFTConfig vbft;
        private DBFTConfig dbft;
        private SOLOConfig solo;

        public string[] SeedList { get => seedList; set => seedList = value; }
        public string ConsensusType { get => consensusType; set => consensusType = value; }
        public VBFTConfig VBFT { get => vbft; set => vbft = value; }
        public DBFTConfig DBFT { get => dbft; set => dbft = value; }
        public SOLOConfig SOLO { get => solo; set => solo = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class VBFTConfig
    {
        private UInt32 n;
        private UInt32 c;
        private UInt32 k;
        private UInt32 l;
        private UInt32 bolckMsgDelay;
        private UInt32 hashMsgDelay;
        private UInt32 peerHandshakeTimeout;
        private UInt32 maxBlockChangeView;
        private UInt32 minInitStake;
        private string adminOntID;
        private string vrfValue;
        private string vrfProof;
        private VBFTPeerStakeInfo[] peers;

        public uint N { get => n; set => n = value; }
        public uint C { get => c; set => c = value; }
        public uint K { get => k; set => k = value; }
        public uint L { get => l; set => l = value; }
        public uint BolckMsgDelay { get => bolckMsgDelay; set => bolckMsgDelay = value; }
        public uint HashMsgDelay { get => hashMsgDelay; set => hashMsgDelay = value; }
        public uint PeerHandshakeTimeout { get => peerHandshakeTimeout; set => peerHandshakeTimeout = value; }
        public uint MaxBlockChangeView { get => maxBlockChangeView; set => maxBlockChangeView = value; }
        public uint MinInitStake { get => minInitStake; set => minInitStake = value; }
        public string AdminOntID { get => adminOntID; set => adminOntID = value; }
        public string VrfValue { get => vrfValue; set => vrfValue = value; }
        public string VrfProof { get => vrfProof; set => vrfProof = value; }
        public VBFTPeerStakeInfo[] Peers { get => peers; set => peers = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class VBFTPeerStakeInfo
    {
        #region config/config.go

        private UInt32 index;
        private string peerPubkey;
        private string address;
        private UInt64 initPos;

        public uint Index { get => index; set => index = value; }
        public string PeerPubkey { get => peerPubkey; set => peerPubkey = value; }
        public string Address { get => address; set => address = value; }
        public ulong InitPos { get => initPos; set => initPos = value; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class DBFTConfig
    {
        private int getBlockTime;
        private string[] bookKeepers;

        public int GetBlockTime { get => getBlockTime; set => getBlockTime = value; }
        public string[] BookKeepers { get => bookKeepers; set => bookKeepers = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class SOLOConfig
    {
        private int getBlockTime;
        private string[] bookKeepers;

        public int GetBlockTime { get => getBlockTime; set => getBlockTime = value; }
        public string[] BookKeepers { get => bookKeepers; set => bookKeepers = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class CommonConfig
    {
        private int logLevel;
        private string nodeType;
        private Boolean enableEventLog;
        private Dictionary<string, Int64> systemFee;
        private UInt64 gasLimit;
        private UInt64 gasPrice;
        private string dataDir;

        public int LogLevel { get => logLevel; set => logLevel = value; }
        public string NodeType { get => nodeType; set => nodeType = value; }
        public Boolean EnableEventLog { get => enableEventLog; set => enableEventLog = value; }
        public Dictionary<string, long> SystemFee { get => systemFee; set => systemFee = value; }
        public ulong GasLimit { get => gasLimit; set => gasLimit = value; }
        public ulong GasPrice { get => gasPrice; set => gasPrice = value; }
        public string DataDir { get => dataDir; set => dataDir = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class ConsensusConfig
    {
        private Boolean enableConsensus;
        private int maxTxInBlock;

        public Boolean EnableConsensus { get => enableConsensus; set => enableConsensus = value; }
        public int MaxTxInBlock { get => maxTxInBlock; set => maxTxInBlock = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class P2PRsvConfig
    {
        private string[] reservedPeers;
        private string[] maskPeers;

        public string[] ReservedPeers { get => reservedPeers; set => reservedPeers = value; }
        public string[] MaskPeers { get => maskPeers; set => maskPeers = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class P2PNodeConfig
    {
        private Boolean reservedPeersOnly;
        private P2PRsvConfig reservedCfg;
        private UInt32 networkMagic;
        private UInt32 networkId;
        private string networkName;
        private int nodePort;
        private int nodeConsensusPort;
        private Boolean dualPortSupport;
        private Boolean isTLS;
        private string certPath;
        private string keyPath;
        private string caPath;
        private int httpInfoPort;
        private int maxHdrSyncReqs;
        private int maxConnInBound;
        private int maxConnOutBound;
        private int maxConnInBoundForSingleIP;

        public Boolean ReservedPeersOnly { get => reservedPeersOnly; set => reservedPeersOnly = value; }
        public P2PRsvConfig ReservedCfg { get => reservedCfg; set => reservedCfg = value; }
        public uint NetworkMagic { get => networkMagic; set => networkMagic = value; }
        public uint NetworkId { get => networkId; set => networkId = value; }
        public string NetworkName { get => networkName; set => networkName = value; }
        public int NodePort { get => nodePort; set => nodePort = value; }
        public int NodeConsensusPort { get => nodeConsensusPort; set => nodeConsensusPort = value; }
        public Boolean DualPortSupport { get => dualPortSupport; set => dualPortSupport = value; }
        public Boolean IsTLS { get => isTLS; set => isTLS = value; }
        public string CertPath { get => certPath; set => certPath = value; }
        public string KeyPath { get => keyPath; set => keyPath = value; }
        public string CaPath { get => caPath; set => caPath = value; }
        public int HttpInfoPort { get => httpInfoPort; set => httpInfoPort = value; }
        public int MaxHdrSyncReqs { get => maxHdrSyncReqs; set => maxHdrSyncReqs = value; }
        public int MaxConnInBound { get => maxConnInBound; set => maxConnInBound = value; }
        public int MaxConnOutBound { get => maxConnOutBound; set => maxConnOutBound = value; }
        public int MaxConnInBoundForSingleIP { get => maxConnInBoundForSingleIP; set => maxConnInBoundForSingleIP = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class RpcConfig
    {
        private Boolean enableHttpJsonRpc;
        private int httpJsonPort;
        private int httplocalPort;

        public Boolean EnableHttpJsonRpc { get => enableHttpJsonRpc; set => enableHttpJsonRpc = value; }
        public int HttpJsonPort { get => httpJsonPort; set => httpJsonPort = value; }
        public int HttplocalPort { get => httplocalPort; set => httplocalPort = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class RestfulConfig
    {
        private Boolean enableHttpRestful;
        private int httpRestPort;
        private int httpMaxConnection;
        private string httpCertPath;
        private string httpKeyPath;

        public Boolean EnableHttpRestful { get => enableHttpRestful; set => enableHttpRestful = value; }
        public int HttpRestPort { get => httpRestPort; set => httpRestPort = value; }
        public int HttpMaxConnection { get => httpMaxConnection; set => httpMaxConnection = value; }
        public string HttpCertPath { get => httpCertPath; set => httpCertPath = value; }
        public string HttpKeyPath { get => httpKeyPath; set => httpKeyPath = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class WebSocketConfig
    {
        private Boolean enableHttpWs;
        private int httpWsPort;
        private string httpCertPath;
        private string httpKeyPath;

        public Boolean EnableHttpWs { get => enableHttpWs; set => enableHttpWs = value; }
        public int HttpWsPort { get => httpWsPort; set => httpWsPort = value; }
        public string HttpCertPath { get => httpCertPath; set => httpCertPath = value; }
        public string HttpKeyPath { get => httpKeyPath; set => httpKeyPath = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// config/config.go
    /// </remarks>
    public class GOBConfig
    {
        private GenesisConfig genesis;
        private CommonConfig common;
        private ConsensusConfig consensus;
        private P2PNodeConfig p2PNode;
        private RpcConfig rpc;
        private RestfulConfig restful;
        private WebSocketConfig ws;

        public GenesisConfig Genesis { get => genesis; set => genesis = value; }
        public CommonConfig Common { get => common; set => common = value; }
        public ConsensusConfig Consensus { get => consensus; set => consensus = value; }
        public P2PNodeConfig P2PNode { get => p2PNode; set => p2PNode = value; }
        public RpcConfig Rpc { get => rpc; set => rpc = value; }
        public RestfulConfig Restful { get => restful; set => restful = value; }
        public WebSocketConfig Ws { get => ws; set => ws = value; }
    }
}

namespace GOBLog
{
    public class Logger
    {
        private int level;
        private Logger _logger;
        private FileInfo logFile;

        public int Level { get => level; set => level = value; }
        public Logger logger { get => _logger; set => _logger = value; }
        public FileInfo LogFile { get => logFile; set => logFile = value; }
    }
}
