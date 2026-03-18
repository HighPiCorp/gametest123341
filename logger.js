const Log = Error;

const extractLineNumberFromStack = (stack) => {
  if (!stack) return '?';

  let line = stack.split('\n')[2];
  line = (line.indexOf(' (') >= 0
    ? line.split(' (')[1].substring(0, line.length - 1)
    : line.split('at ')[1]
  );
  return line;
};

Log.prototype.write = (error, messageToPlayer = '') => {
  const suffix = {
    '@': (this.lineNumber
      ? `${this.fileName}:${this.lineNumber}:1`
      : extractLineNumberFromStack(this.stack)
    ),
  };

  error = error.concat([suffix]);

  mp.events.call('client:ui:debug', 'ui.log', error[0].stack, messageToPlayer);
  mp.console.logError(`LOGGER: ${error[0].stack}`);
};

Log.prototype.debug = (message) => {
  mp.events.call('client:ui:debug', 'ui.log', message, '', false);
};

global.logger = {
  error(error, messageToPlayer = '') {
    // if (typeof DEBUGMODE === typeof undefined || !DEBUGMODE) return;

    Log().write(Array.prototype.slice.call([error], 0), messageToPlayer);
  },
  debug(message) {
    Log().debug(message);
  },
};
