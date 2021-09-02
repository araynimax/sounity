fx_version 'cerulean'
game 'gta5'

author 'Arayni <arayni.max@gmail.com>'
description 'A library that helps u playing positional sound'
version '2.0.0'

ui_page 'dist/index.html'
files {
	'dist/index.html',
	'dist/main.js',
	'dist/main.js.map',
	'dist/Newtonsoft.Json.dll',
	'dist/sounity-shared.net.dll',
	'config.ini'
}

client_scripts {
	'dist/sounity-client.net.dll'
}

server_scripts {
	'dist/sounity-server.net.dll'
}
