function Peer() {
  var callStatus;
  var remoteId;
  var remoteDesc;

  var peer;
  var onStream, onCall, onAnswer, onHangup, sendRequest, sendCandidate, sendAnswer, sendHangup;
  var sendCandidateQueue = [];
  var preparePeer = () => {
    var peer = new RTCPeerConnection();
    peer.onicecandidate = evt => {
      if (evt.candidate) {
        if (callStatus === "connected") sendCandidate(remoteId, evt.candidate);
        else sendCandidateQueue.push(evt.candidate);
      }
    };
    peer.onaddstream = evt => onStream(evt.stream);
    return peer;
  };

  var hangupInternal = () => {
    callStatus = remoteId = remoteDesc = null;
    if (peer) peer.close();
    peer = null;
  };

  var call = (id, video, stream) => {
    if (remoteId) return false;
    remoteId = id;
    peer = preparePeer();
    callStatus = "connecting";
    peer.addStream(stream);
    peer.createOffer().then(desc => {
      peer.setLocalDescription(new RTCSessionDescription(desc));
      sendRequest(id, video, desc);
    });
  };

  var answer = (video, stream) => {
    callStatus = "connected";
    peer = preparePeer();
    peer.addStream(stream);
    var sessionDesc = new RTCSessionDescription(remoteDesc);
    peer.setRemoteDescription(sessionDesc);
    peer.createAnswer().then(function (desc) {
      peer.setLocalDescription(new RTCSessionDescription(desc));
      sendAnswer(remoteId, video, desc);
    });
  };

  var hangup = () => {
    sendHangup(remoteId);
    hangupInternal();
  };

  var receiveRequest = (id, video, desc) => {
    remoteId = id;
    callStatus = "connecting";
    remoteDesc = desc;
    onCall(id, video);
  };

  var receiveAnswer = (id, video, desc) => {
    callStatus = "connected";
    peer.setRemoteDescription(new RTCSessionDescription(desc));
    sendCandidateQueue.forEach(c => sendCandidate(remoteId, c));
    sendCandidateQueue = [];
    onAnswer(video);
  };

  var receiveHangup = id => {
    hangupInternal();
    onHangup();
  };

  var queue = [];
  var receiveCandidate = (id, candidate) => {
    if (!peer.remoteDescription || !peer.remoteDescription.type) queue.push(candidate);
    else {
      queue.forEach(c => peer.addIceCandidate(new RTCIceCandidate(c)));
      queue = [];
      peer.addIceCandidate(new RTCIceCandidate(candidate));
    }
  };

  var p = {
    call: call,
    answer: answer,
    hangup: hangup,
    onStream: cb => { onStream = cb; return p; },
    onCall: cb => { onCall = cb; return p; },
    onHangup: cb => { onHangup = cb; return p; },
    onAnswer: cb => { onAnswer = cb; return p; },
    // for signaling
    onSendRequest: cb => { sendRequest = cb; return p; },
    onSendCandidate: cb => { sendCandidate = cb; return p; },
    onSendAnswer: cb => { sendAnswer = cb; return p; },
    onSendHangup: cb => { sendHangup = cb; return p; },
    onReceiveRequest: receiveRequest,
    onReceiveCandidate: receiveCandidate,
    onReceiveAnswer: receiveAnswer,
    onReceiveHangup: receiveHangup,
  };

  return p;
}
