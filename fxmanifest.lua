fx_version 'cerulean'
game 'gta5'

author 'Arayni <arayni.max@gmail.com>'
description 'A library that helps u playing positional sound'
version '0.0.1'

sounity_stream_max_range "100" -- if synced sound is x units away of a player it will get destroyed in that players session

ui_page 'dist/index.html'
files {
	'dist/index.html',
	'dist/main.js',
	'dist/main.js.map',
	'dist/Newtonsoft.Json.dll'
}

client_scripts {
	'dist/sounity-client.net.dll'
}

server_scripts {
	'dist/sounity-server.net.dll'
}
