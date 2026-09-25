import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Dialogs

ApplicationWindow {
    id: root; width: 1100; height: 720; minimumWidth: 760; minimumHeight: 520; visible: true
    title: "SynTools · Prueba técnica Qt"; color: palette.window
    property int section: 0
    property bool dark: false
    palette: Palette { window: dark ? "#17191d" : "#f4f5f7"; windowText: dark ? "#f4f5f7" : "#17191d"; base: dark ? "#202329" : "white"; text: dark ? "white" : "black"; button: dark ? "#343943" : "#e5e8ed"; buttonText: dark ? "white" : "black" }
    Shortcut { sequences: [StandardKey.Open]; onActivated: fileDialog.open() }
    Shortcut { sequence: "Ctrl+F"; onActivated: root.visibility = root.visibility === Window.FullScreen ? Window.Windowed : Window.FullScreen }
    FileDialog { id: fileDialog; title: "Seleccionar archivo"; onAccepted: status.text = "Archivo: " + selectedFile }
    FolderDialog { id: folderDialog; title: "Seleccionar carpeta"; onAccepted: status.text = "Carpeta: " + selectedFolder }
    SplitView {
        anchors.fill: parent
        Rectangle {
            SplitView.preferredWidth: 220; color: root.dark ? "#202329" : "#e8ebf0"
            ColumnLayout { anchors.fill: parent; anchors.margins: 12; Label { text: "SynTools"; font.pixelSize: 26; font.bold: true }
                Repeater { model: ["Inicio", "Herramienta de prueba", "Historial", "Ajustes"]
                    Button { required property int index; required property string modelData; text: modelData; Layout.fillWidth: true; onClicked: root.section = index } }
                Item { Layout.fillHeight: true } Label { text: "Qt 6.11 · QML" }
            }
        }
        ScrollView { SplitView.fillWidth: true; contentWidth: availableWidth
            ColumnLayout { width: parent.width; spacing: 12; anchors.margins: 24
                Label { text: ["Inicio", "Herramienta de prueba", "Historial", "Ajustes"][root.section]; font.pixelSize: 28; font.bold: true }
                Label { visible: root.section === 0; text: "Shell compartida, redimensionable y con UI en español." }
                Label { visible: root.section === 2; text: "Estado vacío · no se almacenan rutas privadas." }
                RowLayout { visible: root.section === 3; Label { text: "Tema" } Button { text: root.dark ? "Oscuro" : "Claro"; onClicked: root.dark = !root.dark } }
                ColumnLayout { visible: root.section === 1; Layout.fillWidth: true
                    Label { text: "Arrastra archivos o usa los selectores. Ctrl+F alterna pantalla completa." }
                    RowLayout { Button { text: "Seleccionar archivo…"; onClicked: fileDialog.open() } Button { text: "Seleccionar carpeta…"; onClicked: folderDialog.open() } }
                    DropArea { Layout.fillWidth: true; height: 70; onDropped: drop => status.text = "Elementos arrastrados: " + drop.urls.length
                        Rectangle { anchors.fill: parent; color: "transparent"; border.color: "#4aa3df"; radius: 8; Label { anchors.centerIn: parent; text: "Zona de drag & drop" } } }
                    Label { text: "Render: 20.000 puntos, zoom y playhead" }
                    Slider { id: zoom; from: 1; to: 12; value: 1; Layout.fillWidth: true }
                    Label { text: "Desplazamiento" }
                    Slider { id: pan; from: 0; to: 1; value: 0; Layout.fillWidth: true }
                    Canvas { id: waveform; Layout.fillWidth: true; height: 240; property real playhead: 0; renderTarget: Canvas.FramebufferObject
                        onPaint: { let c=getContext("2d"); c.fillStyle="#05070a"; c.fillRect(0,0,width,height); c.strokeStyle="#20b8ff"; c.beginPath(); let count=Math.floor(20000/zoom.value); let start=Math.floor((20000-count)*pan.value); let step=Math.max(1,Math.floor(count/width)); for(let i=0;i<count;i+=step){let sample=start+i;let x=i/count*width;let y=height*(.5-Math.sin(sample/21)*(.55+.45*Math.sin(sample/997))*.42); if(i===0)c.moveTo(x,y);else c.lineTo(x,y)} c.stroke(); c.strokeStyle="#ff552f"; c.beginPath(); c.moveTo(playhead*width,0);c.lineTo(playhead*width,height);c.stroke() }
                        Timer { interval: 33; running: true; repeat: true; onTriggered: { waveform.playhead=(waveform.playhead+.005)%1; waveform.requestPaint() } } }
                    Label { text: "Audio PCM → QAudioSink" }
                    RowLayout { Button { text: "Cargar fixture"; enabled: fixtureRoot.length > 0; onClicked: status.text = audioEngine.loadWav(fixtureRoot + "/audio_48k_stereo.wav") ? "Audio cargado" : "Error de audio" } Button { text: "Play"; onClicked: audioEngine.play() } Button { text: "Pausa"; onClicked: audioEngine.pause() } Button { text: "Reanudar"; onClicked: audioEngine.resume() } Button { text: "Stop"; onClicked: audioEngine.stop() } }
                    Label { text: "Vídeo FFmpeg → frame controlado" }
                    Image { Layout.fillWidth: true; Layout.preferredHeight: 360; fillMode: Image.PreserveAspectFit; source: mediaController.frameData }
                    RowLayout { Button { text: "Cargar 1080p"; enabled: fixtureRoot.length > 0; onClicked: mediaController.source = fixtureRoot + "/video_1080p.mp4" } Button { text: "Play"; onClicked: mediaController.play() } Button { text: "Pausa"; onClicked: mediaController.pause() } Slider { from: 0; to: 6; value: mediaController.position; onMoved: mediaController.seek(value); Layout.fillWidth: true } }
                    Label { id: status; text: "Preparado" }
                }
            }
        }
    }
}
