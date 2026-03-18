let esptoggle = 0;
let myalvl = 0;

mp.keys.bind(Keys.VK_F12, false, () => {
  if (!global.loggedin || localplayer.getVariable('IS_ADMIN') !== true) return;
  myalvl = localplayer.getVariable('ALVL');
  if (esptoggle == 6) esptoggle = 0;
  else esptoggle++;
  if (esptoggle == 0) mp.game.graphics.notify('ESP: ~r~Disabled');
  else if (esptoggle == 1) mp.game.graphics.notify('ESP: ~g~ID Players');
  else if (esptoggle == 2) mp.game.graphics.notify('ESP: ~g~ID Vehicles');
  else if (esptoggle == 3) mp.game.graphics.notify('ESP: ~g~Players & Vehicles');
  else if (esptoggle == 4) mp.game.graphics.notify('ESP: ~g~Vehicles & Healths');
  else if (esptoggle == 5) mp.game.graphics.notify('ESP: ~g~PEDS');
  else if (esptoggle == 6) mp.game.graphics.notify('ESP: ~g~Zones');
});

// mp.events.add("entityControllerChange", (entity, newController) => {
// 	mp.gui.chat.push(`${entity.type}(ClientID: ${entity.id}) has switched to a new controller [${newController ? newController.name : 'Nobody'}]`);
// });
//
// mp.events.add('incomingDamage', (sourceEntity, sourcePlayer, targetEntity, weapon, boneIndex, damage) => {
// 	mp.gui.chat.push(`INCOMING (${sourceEntity.type} ${sourceEntity.id}) ${sourcePlayer.name} ${targetEntity.name} --- DMG: ${damage}`);
// });
//
// mp.events.add('outgoingDamage', (sourceEntity, sourcePlayer, targetEntity, weapon, boneIndex, damage) => {
// 	mp.gui.chat.push(`OUTGOING (${sourceEntity.name} ${sourceEntity.type} ${sourceEntity}) ${sourcePlayer.name} ${targetEntity.name} --- DMG: ${damage} BONE: ${boneIndex}`);
// });

// mp.events.add('playerEnterVehicle', (vehicle, seat) => {
// 	vehicle.invincibility = false;
// })
//
// mp.events.add('playerExitVehicle', (vehicle, seat) => {
// 	vehicle.invincibility = false;
// })
//
// mp.events.add('entityStreamIn', (vehicle, seat) => {
// 	vehicle.invincibility = false;
// })

mp.events.add('render', () => {
  if (!global.loggedin || localplayer.getVariable('IS_ADMIN') !== true) return;
  if (esptoggle >= 1) {
    try {
      let position;
      if (esptoggle == 1 || esptoggle == 3) {
        mp.players.forEachInStreamRange((player) => {
          if (player.handle !== 0 && player !== mp.players.local) {

              position = player.position;
              if (player.getVariable('IS_ADMIN')) {
                mp.game.graphics.drawText(`${player.name} (${player.remoteId})\nHP: ${player.health.toFixed(2)}\nArmour: ${player.armour.toFixed(2)} `, [position.x, position.y, position.z + 1.5], {
                  scale: [0.3, 0.3],
                  outline: true,
                  color: [255, 0, 0, 255],
                  font: 4,
                });
              } else {
                mp.game.graphics.drawText(`${player.name} (${player.remoteId})\nHP: ${player.health.toFixed(2)}\nArmour: ${player.armour.toFixed(2)}`, [position.x, position.y, position.z + 1.5], {
                  scale: [0.3, 0.3],
                  outline: true,
                  color: [255, 255, 255, 255],
                  font: 4,
                });
              }

          }
        });
      }
      if (esptoggle == 2 || esptoggle == 3) {
        mp.vehicles.forEachInStreamRange((vehicle) => {
          if (vehicle.handle !== 0 && vehicle !== mp.players.local) {
            position = vehicle.position;
            mp.game.graphics.drawText(`${mp.game.vehicle.getDisplayNameFromVehicleModel(vehicle.model)} (${vehicle.getNumberPlateText()} | ${vehicle.remoteId})`, [position.x, position.y, position.z - 0.5], {
              scale: [0.3, 0.3],
              outline: true,
              color: [255, 255, 255, 255],
              font: 4,
            });
          }
        });
      }
      if (esptoggle == 4) {
        mp.vehicles.forEachInStreamRange((vehicle) => {
          if (vehicle.handle !== 0 && vehicle !== mp.players.local) {
            position = vehicle.position;
            mp.game.graphics.drawText(`${mp.game.vehicle.getDisplayNameFromVehicleModel(vehicle.model)} (${vehicle.getNumberPlateText()} | ${vehicle.remoteId} | Health BODY: ${vehicle.getBodyHealth()} | Health BODY2: ${vehicle.getBodyHealth2()} | Health Engine: ${vehicle.getEngineHealth()} | Health PetrolTank: ${vehicle.getPetrolTankHealth()})`, [position.x, position.y, position.z - 0.5], {
              scale: [0.3, 0.3],
              outline: true,
              color: [255, 255, 255, 255],
              font: 4,
            });
          }
        });
      }
      if (esptoggle == 5) {
        mp.peds.forEachInStreamRange((ped) => {
          position = ped.getCoords(true);
          var remoteid = ped.getPedIndexFromIndex();
          mp.game1.graphics.drawText(`ID: ~b~${remoteid}~w~\nModel: ${ped.getModel()}\nHeading: ${ped.getHeading().toFixed(4)}\nPosition: ${position.x.toFixed(4)}, ${position.y.toFixed(4)}, ${position.z.toFixed(4)}`, [position.x, position.y, position.z-0.2], {
            scale: [0.35, 0.35],
            outline: true,
            color: [255, 255, 255, 255],
            font: 4
          });
        });
      }

      if (esptoggle == 6) {
        zones.forEach((zone) => {
          let r = 255;
          let g = 0;
          let b = 0;
          let a = 255;

          if (zone.type === "safeZone") {
            r = 0;
            g = 255;
            b = 0;
          }

          if (zone.type === "BPQUEST") {
            r = 0;
            g = 0;
            b = 255;
          }

          mp.zones.drawZone(zone.vectors, zone.height, r, g, b, a);
        });

        try {
          if (preZones !== null && preZones.length > 0) {
            preZones.forEach((zone) => {
              let r = 255;
              let g = 0;
              let b = 0;
              let a = 255;

              mp.zones.drawZone(zone.vectors, zone.height, r, g, b, a);
            });
          }
        } catch (e) {
          logger.error(e);
        }
      }
    } catch (e) { logger.error() }
  }
});
