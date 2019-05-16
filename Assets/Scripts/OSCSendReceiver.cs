﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class OSCSendReceiver : MonoBehaviour, OSCTransmitter
{
    // ================= PUBLIC MEMBERS AND METHODS =================== //

    // tell listeners which port to communicate over
    public ushort OSCPort;
    // TODO refactor to use a text file in streaming assets
    public string[] hosts;


    // send a message to all our listeners
    public bool SendMessage( string address, params object[] args )
    {
        if( mySenders.Count == 0 ) return false;

        SharpOSC.OscMessage newMessage = new SharpOSC.OscMessage( address, args );
        foreach( SharpOSC.UDPSender sender in mySenders.Values )
        {
            sender.Send( newMessage );
        }

        return true;
    }

    // provide a callback for what to do when we receive a message with address address
    public void ListenForMessage( string address, Action<List<object>> callback )
    {
        myOSCResponders[address] = callback;
    }

    public string GenerateChucKCode( string sendArrayName, string receiveName )
    {
        string chuckCode = string.Format( @"
            OscSend {0}[{2}];
        ", sendArrayName, receiveName, mySenders.Count );

        int i = 0;
        foreach( string ipAddress in mySenders.Keys )
        {
            chuckCode += string.Format( @"
            {3}[{0}].setHost( ""{2}"", {1} );
            ", i, OSCPort, ipAddress, sendArrayName );
            i++;
        }

        chuckCode += string.Format( @"
            OscRecv {0};
            {1} => {0}.port;
            {0}.listen();
        ", receiveName, OSCPort );


        return chuckCode;
    }







    // ================== PRIVATE ========================= //


    private Dictionary<string, SharpOSC.UDPSender> mySenders;
    private SharpOSC.UDPListener myListener;
    private Dictionary<string, Action<List<object>>> myOSCResponders;
    private Queue<Tuple<string, List<object>>> myOSCIncomingMessages;
    private string myIP = "";


    // init data structures
    void Awake()
    {
        mySenders = new Dictionary<string, SharpOSC.UDPSender>();
        myOSCResponders = new Dictionary<string, Action<List<object>>>();
        myOSCIncomingMessages = new Queue<Tuple<string, List<object>>>();
    }

    void Start()
    {
        // make senders
        foreach( string host in hosts )
        {
            mySenders[host] = new SharpOSC.UDPSender( host, OSCPort );
            if( myIP == "" )
            {
                myIP = GetMyIP( mySenders[host] );
            }
        }

        // start OSC listener on port OSCPort
        // define the callback
        SharpOSC.HandleOscPacket listenerCallback = delegate ( SharpOSC.OscPacket packet )
        {
            // get message
            SharpOSC.OscMessage messageReceived = (SharpOSC.OscMessage)packet;

            // send message along to be processed on the main thread in Update()
            myOSCIncomingMessages.Enqueue( Tuple.Create( messageReceived.Address, messageReceived.Arguments ) );
        };


        // set up the listener callback
        myListener = new SharpOSC.UDPListener( OSCPort, listenerCallback );


        // find my IP
        Debug.Log( "my IP is " + myIP );
        if( myIP == "" )
        {
            Debug.Log( "uhh, what's my IP?" );
        }

        // blast out my IP forever, in case anyone has to restart chuck
        InvokeRepeating( "SendMyIP", 1, 1 );
    }

    void SendMyIP()
    {
        SendMessage( "/__serverIP__", myIP );
    }

    string GetMyIP( SharpOSC.UDPSender exampleSender )
    {
        string localIP;
        using( Socket socket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, 0 ) )
        {
            socket.Connect( exampleSender.Address, 65530 );
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
        }

        return localIP;
    }

    void Update()
    {
        // while we have messages
        while( myOSCIncomingMessages.Count > 0 )
        {
            // fetch messages
            Tuple<string, List<object>> oscMessage = myOSCIncomingMessages.Dequeue();

            // route messages
            // check if we know this address
            if( myOSCResponders.ContainsKey( oscMessage.Item1 ) )
            {
                // send the address along to the responder
                myOSCResponders[oscMessage.Item1]( oscMessage.Item2 );
            }
        }
    }

    void OnApplicationQuit()
    {
        // close OSC callback / listener
        myListener.Close();

        // close OSC senders
        foreach( string host in mySenders.Keys )
        {
            mySenders[host].Close();
        }

    }

}

