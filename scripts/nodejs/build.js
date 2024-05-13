const fs = require('fs');
const { resolve } = require('path');
const { runQueue } = require('scrapty');

const generateIndexJson = {
    label: 'Generate index.json file.',
    task: async (next) => {
        const dataPath = resolve(__dirname, '..', '..', 'data');
        const indexPath = resolve(__dirname, '..', '..', 'index.json');

        console.log('Reading files...');
        const files = await fs.promises.readdir(dataPath);
        const jsonFiles = await files
            .filter((file) => file.endsWith('.json') && !file.startsWith('_'))
            .map((file) => encodeURI(['https://lavaleak.github.io/another-sudoku/data', file].join('/')));

        console.log(`Files: ${jsonFiles.map((f)=>`\x1b[0;32m\n${f}`)}\x1b[0m`);
        const contents = JSON.stringify({ data: jsonFiles }, null, 2);

        console.log('Writing index.json file...');
        const writeStream = fs.createWriteStream(indexPath);
        writeStream.write(contents);

        console.log('index.json generated with success!');

        next();
    },
};

runQueue([generateIndexJson]);
