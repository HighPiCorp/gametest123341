mp.events.add('CLIENT::vehicle:create', (vehicle, x, y, z, rx, ry, rz, spawnIndex = 0, dimension = 0) => {
  if (vehicle != null && vehicle.type !== "vehicle") return;
  if (vehicle && mp.vehicles.exists(vehicle)) {
    let timeOut = 1000;

    //if (spawnIndex > 0) timeOut = 1000 * spawnIndex;

    setTimeout(() => {
      if (vehicle != null && vehicle.type !== "vehicle") return;
      if (vehicle && mp.vehicles.exists(vehicle)) {
        if (dimension !== 0) vehicle.dimension = dimension;
        vehicle.position = new mp.Vector3(x, y, z);
        vehicle.rotation = new mp.Vector3(rx, ry, rz);
        vehicle.setOnGroundProperly();
      }

      //mp.console.logInfo(`CLIENT::vehicle:create: ${JSON.stringify(vehicle)}`);
    }, timeOut);
  }
});
