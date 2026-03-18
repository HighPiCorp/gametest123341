// function isVehicleAccessAP(a) {
//   return [2006918058].indexOf(a.model) === -1; // белый список, указывать цифры из чата через запятую
// }
//
// let autopilotStart = !1;
// let autopilotPoint = null;
// let autopilotInterval = null;
// const autoPilotSpeed = 35;
//
// mp.keys.bind(0x58, false, () => { // X key
//   if (mp.gui.cursor.visible || global.isChatOpen || global.disableKeys) return; // Отключи меня если ты не используешь RedAge
//   const a = localplayer.vehicle;
//   if (localplayer.vehicle.getPedInSeat(-1) !== localplayer.handle) return; // Проверка, водителю доступно другим нет
//   if (autopilotStart) {
//     const a = localplayer.vehicle;
//     return a && (localplayer.clearTasks(), localplayer.taskVehicleTempAction(a.handle, 27, 1e4)), autopilotPoint = null, autopilotStart = !1, void clearInterval(autopilotInterval);
//   }
//   if (a == null) return;
//
//   const vehicleName = a.getModel();
//   //mp.console.logInfo(`vehname: ${vehicleName}`); // Номер модели в чат что бы узнать какой он  ив писать в белый список(отключи меня после завершения настройки)
//
//   if (isVehicleAccessAP(a)) return; // отказ автопилота
//
//   const engine = a.getIsEngineRunning();
//   if (engine == false) return mp.game.graphics.notify('Двигатель не заведен.'); // проверка двигателя
//
//   const b = mp.game.invoke('0x1DD1F58F493F1DA5');
//   const c = mp.game.invoke('0x186E5D252FA50E7D');
//   const d = mp.game.invoke('0x1BEDE233E6CD2A1F', c);
//   const e = mp.game.invoke('0x14F96AA50D6FBEA7', c);
//
//   for (let a = d; mp.game.invoke('0xA6DB27D19ECBB7DA', a) != 0; a = e) {
//     if (mp.game.invoke('0xBE9B0959FFD0779B', a) == 4 && !!b) {
//       autopilotPoint = mp.game.ui.getBlipInfoIdCoord(a);
//       break;
//     }
//   }
//   return autopilotPoint == null ? void mp.game.graphics.notify('Автопилот: Для начала пути необходимо указать точку на карте.') : void (!autopilotStart && (mp.game.graphics.notify('Автопилот: Точка указана, маршрут построен, начинаем движение.'), localplayer.taskVehicleDriveToCoord(a.handle, autopilotPoint.x, autopilotPoint.y, autopilotPoint.z, autoPilotSpeed, 1, 1, 2883621, 30, 1), autopilotStart = !0, clearInterval(autopilotInterval), autopilotInterval = setInterval(() => {
//     if (!autopilotStart) return void clearInterval(autopilotInterval);
//     const a = localplayer.vehicle;
//     return a ? mp.game.system.vdist(localplayer.position.x, localplayer.position.y, localplayer.position.z, autopilotPoint.x, autopilotPoint.y, autopilotPoint.z) < 15 ? (localplayer.clearTasks(), a && localplayer.taskVehicleTempAction(a.handle, 27, 1e4), autopilotPoint = null, autopilotStart = !1, clearInterval(autopilotInterval), void mp.game.graphics.notify('Автопилот: Вы достигли места назначения!')) : void 0 : (a && (localplayer.clearTasks(), localplayer.taskVehicleTempAction(a.handle, 27, 1e4)), autopilotStart = !1, void clearInterval(autopilotInterval));
//   }, 300)));
// });
