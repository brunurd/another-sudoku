const fs = require("node:fs");
const { open } = require("node:fs/promises");
const crypto = require('node:crypto');
const { resolve } = require("path");
const { runQueue } = require("scrapty");

const GREEN_CONSOLE_COLOR = "\x1b[0;32m";
const NEUTRAL_CONSOLE_COLOR = "\x1b[0m";

const generateHash = (str) => {
  const key = crypto.randomBytes(32).toString('base64');
  const iv = crypto.randomBytes(12).toString('base64');
  const cipher = crypto.createCipheriv(
    "aes-256-gcm", 
    Buffer.from(key, 'base64'), 
    Buffer.from(iv, 'base64')
  );
  let ciphertext = cipher.update(str, 'utf8', 'base64');
  ciphertext += cipher.final('base64');
  return ciphertext;
};

const generateIndexJson = {
  label: "Generate index.json file.",
  task: async (next) => {
    const projectRoot = resolve(__dirname, "..", "..");
    const levelsPath = resolve(projectRoot, "data", "levels");
    const indexPath = resolve(projectRoot, "index.json");
    const godotDataFolder = resolve(projectRoot, "Anothr Sudoku", "Data");
    const godotIndexPath = resolve(godotDataFolder, "index.json");
    const godotLevelsPath = resolve(godotDataFolder, "levels");

    console.log("Reading files...");
    const files = await fs.promises.readdir(levelsPath);
    const jsonFiles = await files
      .filter((file) => file.endsWith(".json") && !file.startsWith("_"))
      .map((file) => {
        return {
          file: resolve(levelsPath, file),
          url: encodeURI(
            `https://lavaleak.github.io/anothr-sudoku/data/levels/${file}`
          ),
        };
      });

    console.log(
      `Files: ${jsonFiles.map(
        (f) => `${GREEN_CONSOLE_COLOR}\n${f.url}`
      )}${NEUTRAL_CONSOLE_COLOR}`
    );

    console.log("Writing level files into Godot project...");
    if (!fs.existsSync(godotLevelsPath)) {
      fs.mkdirSync(godotLevelsPath);
    }
    for (let jsonFile of jsonFiles) {
      const regexMatch = /.*\/(.*).json$/.exec(jsonFile.url);
      if (regexMatch) {
        console.log(
          `Writing level: ${GREEN_CONSOLE_COLOR}${jsonFile.file}${NEUTRAL_CONSOLE_COLOR} to Godot project...`
        );
        const godotLevelPath = resolve(
          godotLevelsPath,
          `${regexMatch[1]}.json`
        );
        const godotLevelWriteStream = fs.createWriteStream(godotLevelPath);
        const file = await open(jsonFile.file);
        const contents = (await file.readFile()).toString();
        godotLevelWriteStream.write(contents);
        godotLevelWriteStream.close();
      }
    }

    console.log("Generating unique build hash...");
    const hash = generateHash(
      JSON.stringify({
        timestamp: Date.now(),
        files: jsonFiles.join(';'),
      }),
    );

    const contents = JSON.stringify(
      { hash, data: jsonFiles.map((f) => f.url) },
      null,
      2
    );

    console.log("Writing index.json file...");
    const indexWriteStream = fs.createWriteStream(indexPath);
    indexWriteStream.write(contents);
    indexWriteStream.close();

    console.log("Writing index.json file to Godot project...");
    const godotIndexWriteStream = fs.createWriteStream(godotIndexPath);
    godotIndexWriteStream.write(contents);
    godotIndexWriteStream.close();

    console.log("index.json generated with success!");

    next();
  },
};

runQueue([generateIndexJson]);
