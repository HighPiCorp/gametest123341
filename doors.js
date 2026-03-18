mp.events.add('setDoorLocked', function (model, x, y, z, locked, angle, isBarrier) {
    try {
        if(isBarrier)
        {
            object = mp.game.object.getClosestObjectOfType(x, y, z, 2, model, false, false, false);
            if(locked)
            {
                object.setRotation(0,90,0,true);
            }
            else
            { 
                object.setRotation(0,0,0,true);
            }
            return;
        }
        mp.game1.invoke("0x428CA6DBD1094446", mp.game.object.getClosestObjectOfType(x, y, z, 2, model, false, false, false));
        mp.game1.object.doorControl(model, x, y, z, locked, 0, 0, angle);
    } catch (e) {
        logger.error(e);
    }
});