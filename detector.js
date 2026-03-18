var IsSound = false;
var IsMarker = false;
var DetectorMarker = -1;
var localplayer = mp.players.local;

mp.events.add('render', () => {
    var interval = -1;
    DetectorMarker = -1;
    if(localplayer.vehicle)return;

    if(!localplayer.hasVariable('CURRECTCRAFTCOLSHAPE'))return;

    if(!localplayer.hasVariable('DigScannerInHand'))return;

    var ColData = localplayer.getVariable('CURRECTCRAFTCOLSHAPE');
    var DetectorLevel = localplayer.getVariable('DigScannerInHand');

    if(DetectorLevel == undefined || DetectorLevel == null || DetectorLevel == -1)return;
    if(ColData == undefined || ColData == null)return;

    var ColPosition = new mp.Vector3(ColData.x, ColData.y, ColData.z);
    var distance =  mp.Vector3.getDistanceBetweenPoints3D(ColPosition, localplayer.position);

    if(distance >= 0 && distance < 2)
    {
        DetectorMarker = 0;
        interval = -1;
    }
    else if(distance >= 2 && distance < 10)
    {
        DetectorMarker = 1;
        interval = 333;
    }
    else if(distance >= 10 && distance < 15)
    {
        DetectorMarker = 2;
        interval = 500;
    }
    else if(distance >= 15 && distance < 20)
    {
        DetectorMarker = 3;
        interval = 1000;
    }
    else if(distance >= 20 && distance < 35 && DetectorLevel > 1)
    {
        DetectorMarker = 4;
        interval = 1500;
    }
    else if(distance >= 35 && distance < 50 && DetectorLevel > 1)
    {
        DetectorMarker = -1;
        interval = 2000;
    }
    else if(distance >= 50 && distance < 70 && DetectorLevel > 2)
    {
        DetectorMarker = -1;
        interval = 2500;
    }
    else if(distance >= 70 && distance < 100 && DetectorLevel > 2)
    {
        DetectorMarker = -1;
        interval = 3000;
    }
    if(interval != -1)
        playDetectorSound(interval);
});
mp.events.add('render', () => {

    switch (DetectorMarker)
    {
        case 0:
            drawDetectorMarker(0,255,0);
            break;
        case 1:
            drawDetectorMarker(128,255,0);
            break;
        case 2:
            drawDetectorMarker(255,255,0);
            break;
        case 3:
            drawDetectorMarker(255,128,0);
            break;
        case 4:
            drawDetectorMarker(255,0,0);
            break;
    }
});

function drawDetectorMarker(r,g,b)
{
    //detectorPos = localplayer.getBoneCoords(18905, 0.13, 0, 0.02);
    mp.game.graphics.drawMarker(
        27,
        localplayer.position.x, localplayer.position.y, localplayer.position.z - 0.9,
        0, 0, 0,
        0.13, 0, 0.02,
        1, 1, 1,
        r, g, b, 125,
        false, false, 1,
        true, null, null, false);
}

function playDetectorSound(interval)
{
    if(IsSound)return;
    global.policeGarage.execute(`client_playMusic('package://cef/sounds/craft_detector.mp3', 0.1)`);
    IsSound = true;
    setTimeout(() => {
        IsSound = false;
    }, interval);
}