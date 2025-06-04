let router;
let editViewer = null;
let browseViewer = null;
let groupUid = "uniqueID";

class MyMobileViewer extends MyViewer {

    constructor(options) {
        super();
        this.initViewers();
    }

    initViewers(){
        initBrowseViewer();
        initEditViewer();
        bindEvents();
    }

    // override
    getDocument() {
        let doc = editViewer.currentDocument;
        if (!doc) {
            doc = Dynamsoft.DDV.documentManager.createDocument();
            editViewer.openDocument(doc.uid);
        }
        return doc;
    }

    async saveToPng(index, settings) {
        const doc = this.getDocument();
        const file = await doc?.saveToPng(index, settings);
        return await MyViewerApp.fileToBase64(file);
    }

    async saveCurrentToPng(settings) {
        if (editViewer.getPageCount() <= 0) {
            return "";
        }
        return await this.saveToPng(editViewer.getCurrentPageIndex(), settings);
    }
}

function initEditViewer(){
    const config = {
        type: Dynamsoft.DDV.Elements.Layout,
        flexDirection: "column",
        className: "ddv-edit-viewer-mobile",
        children: [
            {
            type: Dynamsoft.DDV.Elements.Layout,
            className: "ddv-edit-viewer-header-mobile",
            children: [
                {
                // Add a "Back" buttom to header and bind click event to go back to the perspective viewer
                // The event will be registered later.
                type: Dynamsoft.DDV.Elements.Button,
                className: "ddv-button-back",
                events:{
                    click: "back"
                }
                },
                Dynamsoft.DDV.Elements.Pagination
            ],
            },
            Dynamsoft.DDV.Elements.MainView,
            {
            type: Dynamsoft.DDV.Elements.Layout,
            className: "ddv-edit-viewer-footer-mobile",
            children: [
                Dynamsoft.DDV.Elements.DisplayMode,
                Dynamsoft.DDV.Elements.RotateLeft,
                Dynamsoft.DDV.Elements.Crop,
                Dynamsoft.DDV.Elements.Filter,
                Dynamsoft.DDV.Elements.Undo,
                Dynamsoft.DDV.Elements.Delete,
                {   
                    // Bind event for "PerspectiveAll" button to show the edit viewer
                    // The event will be registered later.
                    type: Dynamsoft.DDV.Elements.Button,
                    className: "ddv-load-image",
                    events:{
                        click: "loadFile"
                    }
                },
            ],
            },
        ],
    };
    editViewer = new Dynamsoft.DDV.EditViewer({
        groupUid: groupUid,
        container: "container",
        uiConfig: config,
    });
    editViewer.hide();
}

function initBrowseViewer(){
    browseViewer = new Dynamsoft.DDV.BrowseViewer({
        groupUid: groupUid,
        container: "container"
    });
}

function bindEvents(){
    editViewer.on("back",() => {
        switchViewer(0,1);
    });

    editViewer.on("loadFile",() => {
        MyViewerApp.invokeDotNet('loadFile', 'true', '');
    });
}

function showEditor(){
    switchViewer(1,0);
}

// Define a function to control the viewers' visibility
function switchViewer(e,b) {
    editViewer.hide();
    browseViewer.hide();
    if(e) editViewer.show();
    if(b) browseViewer.show();
};

