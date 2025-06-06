class MyViewer {

    // purely virtual function
    getDocument() {
        throw new Error('getDocument() must be implemented by subclass');
    }
    
    async loadSource(base64Image) {
        const doc = this.getDocument();
        
        await doc.loadSource(MyViewerApp.base64toBlob(base64Image));
    }

    async loadDocument(url) {
        const doc = this.getDocument();

        
        const response = await fetch(url, {
            method: "GET"
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const image = await response.arrayBuffer();
        await doc?.loadSource(new Blob([image], { type: response.headers.get("content-type") }));
    }

    async saveToPng(index, settings) {
        const doc = this.getDocument();
        const file = await doc?.saveToPng(index, settings);
        return await MyViewerApp.fileToBase64(file);
    }

    async saveToJpeg(index, settings) {
        const doc = this.getDocument();
        const file = await doc?.saveToJpeg(index, settings);
        return await MyViewerApp.fileToBase64(file);
    }

    async saveAsTiff(indicies, settings) {
        const doc = this.getDocument();

        indicies = indicies ?? [...Array(doc?.pages.length).keys()];
        if (indicies.length === 0) {
            return "";
        }
        if (settings) {
            const file = await doc?.saveToTiff(indicies, settings);
            return await MyViewerApp.fileToBase64(file);
        }
        else {
            const file = await doc?.saveToTiff(indicies);
            return await MyViewerApp.fileToBase64(file);
        }
    }

    async saveAllAsTiff(settings) {
        return await this.saveAsTiff(null, settings);
    }

    async saveAsPdf(indicies, settings) {
		console.log(settings);
        const doc = this.getDocument();

        indicies = indicies ?? [...Array(doc?.pages.length).keys()];
        if (indicies.length === 0) {
            return "";
        }
        if (settings) {
            const file = await doc?.saveToPdf(indicies, settings);
            return await MyViewerApp.fileToBase64(file);
        }
        else {
            const file = await doc?.saveToPdf(indicies);
            return await MyViewerApp.fileToBase64(file);
        }
    }

    async saveAllAsPdf(settings) {
        return await this.saveAsPdf(null, settings);
    }
	
	async setBarcodeResult(results){
		//results = "[{"BarcodeFormat":2,"BarcodeFormatString":"CODE_128","LocalizationResult":{"ResultPoints":["723, 275","1021, 275","1021, 375","723, 375"],"accompanyingTextBytes":[],"angle":0,"barcodeFormat":3147775,"barcodeFormatString":"OneD","barcodeFormatString_2":"No Barcode Format in group 2","barcodeFormat_2":0,"confidence":100,"documentName":"{E55BE964-BA6D-4B9D-95DE-CA5C4994ADBC}","moduleSize":2,"pageNumber":1,"regionName":"","resultCoordinateType":1,"terminatePhase":32,"x1":723,"x2":1021,"x3":1021,"x4":723,"y1":275,"y2":275,"y3":375,"y4":375},"barcodeBytes":[67,79,68,69,49,50,56],"barcodeFormat":2,"barcodeFormatString":"CODE_128","barcodeFormatString_2":"No Barcode Format in group 2","barcodeFormat_2":0,"barcodeText":"CODE128","detailedResult":{"checkDigitBytes":[],"moduleSize":2,"startCharsBytes":[],"stopCharsBytes":[]},"localizationResult":{"ResultPoints":["723, 275","1021, 275","1021, 375","723, 375"],"accompanyingTextBytes":[],"angle":0,"barcodeFormat":3147775,"barcodeFormatString":"OneD","barcodeFormatString_2":"No Barcode Format in group 2","barcodeFormat_2":0,"confidence":100,"documentName":"{E55BE964-BA6D-4B9D-95DE-CA5C4994ADBC}","moduleSize":2,"pageNumber":1,"regionName":"","resultCoordinateType":1,"terminatePhase":32,"x1":723,"x2":1021,"x3":1021,"x4":723,"y1":275,"y2":275,"y3":375,"y4":375},"results":[{"accompanyingTextBytes":[],"barcodeFormat":2,"barcodeFormatString":"CODE_128","barcodeFormatString_2":"No Barcode Format in group 2","barcodeFormat_2":0,"bytes":[67,79,68,69,49,50,56],"clarity":-1,"confidence":100,"deformation":0,"resultType":0}]},{"BarcodeFormat":1024,"BarcodeFormatString":"CODE_39_EXTENDED","LocalizationResult":{"ResultPoints":["183, 272","564, 273","564, 373","183, 372"],"accompanyingTextBytes":[],"angle":0,"barcodeFormat":3147775,"barcodeFormatString":"OneD","barcodeFormatString_2":"No Barcode Format in group 2","barcodeFormat_2":0,"confidence":57,"documentName":"{E55BE964-BA6D-4B9D-95DE-CA5C4994ADBC}","moduleSize":3,"pageNumber":1,"regionName":"","resultCoordinateType":1,"terminatePhase":32,"x1":183,"x2":564,"x3":564,"x4":183,"y1":272,"y2":273,"y3":373,"y4":372},"barcodeBytes":[67,79,68,69,51,57],"barcodeFormat":1024,"barcodeFormatString":"CODE_39_EXTENDED","barcodeFormatString_2":"No Barcode Format in group 2","barcodeFormat_2":0,"barcodeText":"CODE39","detailedResult":{"checkDigitBytes":[],"moduleSize":3,"startCharsBytes":[42],"stopCharsBytes":[42]},"localizationResult":{"ResultPoints":["183, 272","564, 273","564, 373","183, 372"],"accompanyingTextBytes":[],"angle":0,"barcodeFormat":3147775,"barcodeFormatString":"OneD","barcodeFormatString_2":"No Barcode Format in group 2","barcodeFormat_2":0,"confidence":57,"documentName":"{E55BE964-BA6D-4B9D-95DE-CA5C4994ADBC}","moduleSize":3,"pageNumber":1,"regionName":"","resultCoordinateType":1,"terminatePhase":32,"x1":183,"x2":564,"x3":564,"x4":183,"y1":272,"y2":273,"y3":373,"y4":372},"results":[{"accompanyingTextBytes":[],"barcodeFormat":1024,"barcodeFormatString":"CODE_39_EXTENDED","barcodeFormatString_2":"No Barcode Format in group 2","barcodeFormat_2":0,"bytes":[67,79,68,69,51,57],"clarity":-1,"confidence":57,"deformation":0,"resultType":0}]}]";
		console.log(results);
        const doc = this.getDocument();
        for (var i = 0; i < results.length; i++) {
            var item = results[i];
            let text = item.barcodeText;
            let loc = item.localizationResult;//.points;

            let currentPageId = doc.pages[this.editViewer.getCurrentPageIndex()];
            let pageData = await doc.getPageData(currentPageId);

            // https://www.dynamsoft.com/document-viewer/docs/api/interface/annotationinterface/texttypewriterannotationoptions.html
            let textX = Math.min(loc.x1, loc.x2, loc.x3, loc.x4) / pageData.display.width * pageData.mediaBox.width;
            let textY = Math.min(loc.y1, loc.y2, loc.y3, loc.y4) / pageData.display.height * pageData.mediaBox.height;

            const textTypewriterOptions = {
                x: textX < 0 ? 0 : textX,
                y: textY - 15 < 0 ? 0 : textY - 15,
                textContents: [{ content: text, color: "rgb(255,0,0)" }],
                flags: {
                    print: false,
                    noView: false,
                    readOnly: true,

                }
            }

            // https://www.dynamsoft.com/document-viewer/docs/api/class/annotationmanager.html#createAnnotation
            let textTypewriter = await Dynamsoft.DDV.annotationManager.createAnnotation(currentPageId, "textTypewriter", textTypewriterOptions)
            textTypewriter['name'] = 'overlay';

            // https://www.dynamsoft.com/document-viewer/docs/api/interface/annotationinterface/polygonannotationoptions.html
            const polygonOptions = {
                points: loc.ResultPoints.map(p => {
                    const numbers = p.split(", ").map(Number);
                    return {
                        x: numbers[0] / pageData.display.width * pageData.mediaBox.width,
                        y: numbers[1] / pageData.display.height * pageData.mediaBox.height
                    }
                }),
                borderColor: "rgb(255,0,0)",
                flags: {
                    print: false,
                    noView: false,
                    readOnly: true,

                }
            }

            let polygon = Dynamsoft.DDV.annotationManager.createAnnotation(currentPageId, "polygon", polygonOptions);
            polygon['name'] = 'overlay';
        }
    }

    async clearAnnotations() {
        const doc = this.getDocument();
		if (!doc) {
			alert("Please load a document first.");
			return;
		}

		let currentPageId = doc.pages[this.editViewer.getCurrentPageIndex()];
		let annotations = Dynamsoft.DDV.annotationManager.getAnnotationsByPage(currentPageId);

		if (annotations.length > 0) {
			for (let i = 0; i < annotations.length; i++) {
				// https://www.dynamsoft.com/document-viewer/docs/api/class/annotationmanager.html#deleteannotations

				if (!annotations[i].flattened && annotations[i].name !== 'barcode') {
					await Dynamsoft.DDV.annotationManager.deleteAnnotations([annotations[i].uid]);
				}
			}
		}
	}
}

class MyDesktopViewer extends MyViewer {
    editViewer = null;
    fullFeatureEditViewer = null;
    constructor(options) {
        super();
        let newUiConfig = Dynamsoft.DDV.getDefaultUiConfig("editViewer", { includeAnnotationSet: true });
        if (options.uiConfig === 'desktop-default') {
            newUiConfig = {
                type: Dynamsoft.DDV.Elements.Layout,
                flexDirection: "column",
                className: "ddv-edit-viewer-desktop",
                children: [
                    Dynamsoft.DDV.Elements.MainView,
                    {
                        type: Dynamsoft.DDV.Elements.Pagination,
                        className: "custom-pagination",
                    }
                ],
            };
        }
        let groupUid = "uniqueID";
        let fullFeatureConfig = {
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
                    Dynamsoft.DDV.Elements.Pagination,
                    Dynamsoft.DDV.Elements.Blank
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
                        Dynamsoft.DDV.Elements.Delete
                    ],
                },
            ],
        };
        this.fullFeatureEditViewer = new Dynamsoft.DDV.EditViewer({
            container: "container",
            uiConfig: fullFeatureConfig,
            groupUid: groupUid,
			viewerConfig: {
				scrollToLatest: true,
            },
            thumbnailConfig: {
				scrollToLatest: true,
			}
        });
        this.fullFeatureEditViewer.hide();
        this.fullFeatureEditViewer.on("back",() => {
            this.fullFeatureEditViewer.hide();
            this.editViewer.show();
        });
        this.editViewer = new Dynamsoft.DDV.EditViewer({
            container: "container",
            uiConfig: newUiConfig,
            groupUid: groupUid,
            viewerConfig: {
                canvasStyle: {
                    background: "rgb(255,255,255)"
                },
				scrollToLatest: true,
            },
            thumbnailConfig: {
                visibility: "visible",
                checkboxStyle: {
                    visibility: "hidden",
                },
                pageNumberStyle: {
                    visibility: "hidden",
                },
                canvasStyle: {
                    background: "rgb(244,244,244)"
                },
                selectedPageStyle: {
                    border: "2px solid rgb(153,209,255)",
                    background: "rgb(204,232,255)"
                },
                currentPageStyle: {
                    border: "0px solid blue"
                },
                hoveredPageStyle: {
                    border: "0px solid blue",
                    background: "rgb(229,243,255)"
                },
				scrollToLatest: true,
            },
            annotationConfig: {
                enableContinuousDrawing: true
            }
        });
        
        this.editViewer.displayMode = "single";
        let internalIndexChanged = false;
        this.editViewer.on("currentIndexChanged", (evt) => {
            let selectedIndices = this.editViewer.thumbnail?.getSelectedPageIndices();
            if (!internalIndexChanged) {
                selectedIndices = [];
            }

            if (evt.newIndex != -1 && !selectedIndices.includes(evt.newIndex)) {
                selectedIndices.push(evt.newIndex);
                this.editViewer.thumbnail?.selectPages(selectedIndices);
            }
        });

        this.editViewer.thumbnail?.on("selectedPagesChanged", (evt) => {
            const currentPageIndex = this.editViewer.getCurrentPageIndex();
            if (!evt.newIndices.includes(currentPageIndex) && evt.newIndices.length > 0) {
                internalIndexChanged = true;
                this.editViewer.goToPage(evt.newIndices[evt.newIndices.length - 1]);
                internalIndexChanged = false;
            }
        });
    }

    // override
    getDocument() {
        let doc = this.editViewer.currentDocument;
        if (!doc) {
            doc = Dynamsoft.DDV.documentManager.createDocument();
            this.editViewer.openDocument(doc.uid);
        }
        return doc;
    }

    showFullFeatureEditor(){
        this.editViewer.hide();
        this.fullFeatureEditViewer.show();
    }

    setToolMode(mode) {
        this.editViewer.toolMode = mode;
        return (this.editViewer.toolMode === mode);
    }

    rotateCurrentPage(angle) {
        return this.editViewer.rotate(angle, [this.editViewer.getCurrentPageIndex()]);
    }

    rotateSelectedPages(angle) {
        return this.editViewer.rotate(angle, this.editViewer.thumbnail?.getSelectedPageIndices());
    }

    cropCurrentPage() {
        return this.editViewer.crop(this.editViewer.getCropRect(), [this.editViewer.getCurrentPageIndex()]);
    }

    cropSelectedPages() {
        return this.editViewer.crop(this.editViewer.getCropRect(), this.editViewer.thumbnail?.getSelectedPageIndices());
    }

    undo() {
        return this.editViewer.undo();
    }

    redo() {
        return this.editViewer.redo();
    }

    setFitMode(mode) {
        this.editViewer.fitMode = mode;
        return (this.editViewer.fitMode === mode);
    }

    setAnnotationMode(mode) {
        this.editViewer.toolMode = 'annotation';
        this.editViewer.annotationMode = mode;
        return (this.editViewer.annotationMode === mode);
    }

    deleteCurrentPage() {
        return this.editViewer.currentDocument?.deletePages([this.editViewer.getCurrentPageIndex()]);
    }

    deleteSelectedPages() {
        return this.editViewer.currentDocument?.deletePages(this.editViewer.thumbnail?.getSelectedPageIndices());
    }

    deleteAllPages() {
        return this.editViewer.currentDocument?.deleteAllPages();
    }

    getSelectedPagesCount() {
        return this.editViewer.thumbnail?.getSelectedPageIndices().length;
    }

    getPageCount() {
        return this.editViewer.getPageCount();
    }

    async saveCurrentToPng(settings) {
        if (this.editViewer.getPageCount() <= 0) {
            return "";
        }
        return await this.saveToPng(this.editViewer.getCurrentPageIndex(), settings);
    }

    async saveCurrentToJpeg(settings) {
        if (this.editViewer.getPageCount() <= 0) {
            return "";
        }
        return await this.saveToJpeg(this.editViewer.getCurrentPageIndex(), settings);
    }

    async saveCurrentAsTiff(settings) {
        return await this.saveAsTiff([this.editViewer.getCurrentPageIndex()], settings);
    }

    async saveSelectedAsTiff(settings) {
        return await this.saveAsTiff(this.editViewer.thumbnail?.getSelectedPageIndices(), settings);
    }

    async saveCurrentAsPdf(settings) {
        return await this.saveAsPdf([this.editViewer.getCurrentPageIndex()], settings);
    }

    async saveSelectedAsPdf(settings) {
        return await this.saveAsPdf(this.editViewer.thumbnail?.getSelectedPageIndices(), settings);
    }
}
class MyViewerApp {
    static productkey = "";
    static messageType = "";
    static myViewer = null;

    // Unified function to send messages to .NET
    static sendMessageToDotNet(message) {
        try {
            message = (this.messageType === '') ? message : `${this.messageType}|${message}`;
            if (window.chrome && window.chrome.webview) {
                // For WinForms and WPF (WebView2)
                window.chrome.webview.postMessage(message);
            }
            else if (window.DotNet) {
                // For Blazor, not sure if this is the right way
                DotNet.invokeMethodAsync(blazorAppName, blazorCallbackName, message);
            }
            else if (window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.webwindowinterop) {
                // iOS and MacCatalyst WKWebView
                window.webkit.messageHandlers.webwindowinterop.postMessage(message);
            }
            else if (hybridWebViewHost) {
                // Android WebView
                hybridWebViewHost.sendMessage(message);
            }
            else {
                console.error("Unsupported platform or WebView environment.");
            }
        }
        catch (error) {
            console.error("Error sending message to .NET:", error);
        }
    }

    static invokeDotNet(context, result, error) {
        try {
            this.sendMessageToDotNet(JSON.stringify([context ?? '', result ?? '', error ?? '']));

        } catch (e) {
            console.error(`Error sending back: ${context}:`, e);
        }
    }

    static async initView(options) {
        return await MyViewerApp.createView(options);
    }

    static findFunction(ins, name) {
        if (!ins) {
            if (name === 'initView') {
                return this.initView;
            }
            return null;
        }

        let proto = Object.getPrototypeOf(ins);
        while (proto) {
            if (proto.hasOwnProperty(name)) {
                //console.log(`${name} defined on:`, proto.constructor.name);
                return proto[name];
            }
            proto = Object.getPrototypeOf(proto);
        }
        //console.log(`${name} not found in prototype chain.`);
        return null;
    }

    static async invokeJavaScript(name, params, context) {
        let error = '';
        try {
            context = context ?? '';
            const func = this.findFunction(this.myViewer, name);
            // Check if the function exists and is callable
            if (func) {
                // Decode the base64 string into a JSON array
                const decodedParams = JSON.parse(params);

                // Dynamically call the function with the provided parameters
                const result = await func.bind(this.myViewer)(...decodedParams);

                // If a callback is provided, call it with the result
                if (context) {
                    this.invokeDotNet(context, result ?? "", error);
                }
                return;
            } else {
                error = `Function ${name} is not defined or not callable.`;
                console.error(error);
                this.invokeDotNet(context, '', error);
            }
        } catch (e) {
            console.error(`Error invoking function ${name}:`, e);
            error = e.cause ?? JSON.stringify(e, Object.getOwnPropertyNames(e));
            this.invokeDotNet(context, '', error);
        }
    }

    static base64toBlob(base64Data, contentType = '', sliceSize = 512) {
        const byteCharacters = atob(base64Data);
        const byteArrays = [];

        for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            const slice = byteCharacters.slice(offset, offset + sliceSize);

            const byteNumbers = new Array(slice.length);
            for (let i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            const byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }

        const blob = new Blob(byteArrays, { type: contentType });
        return blob;
    }

    static async fileToBase64(file) {
        if (!file) {
            return Promise.resolve("");
        }
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = () => resolve(reader.result.split(',')[1]); // Extract Base64 part
            reader.onerror = (error) => reject(error);
            reader.readAsDataURL(file);
        });
    };

    static acquireImageFromCamera(mainViewer) {
        const pcCaptureUiConfig = {
            type: Dynamsoft.DDV.Elements.Layout,
            flexDirection: "column",
            className: "ddv-capture-viewer-desktop",
            children: [
                {
                    type: Dynamsoft.DDV.Elements.Layout,
                    className: "ddv-capture-viewer-header-desktop",
                    children: [
                        {
                            type: Dynamsoft.DDV.Elements.CameraResolution,
                            className: "ddv-capture-viewer-resolution-desktop",
                        },
                        Dynamsoft.DDV.Elements.AutoDetect,
                        {
                            type: Dynamsoft.DDV.Elements.Capture,
                            className: "ddv-capture-viewer-capture-desktop",
                        },
                        Dynamsoft.DDV.Elements.AutoCapture,
                        {
                            type: Dynamsoft.DDV.Elements.Button,
                            className: "ddv-button-close position-button-close", // Set the button's icon
                            tooltip: "close viewer", // Set tooltip for the button
                            events: {
                                click: "close", // Set the click event
                            },
                        },
                    ],
                },
                Dynamsoft.DDV.Elements.MainView,
                {
                    type: Dynamsoft.DDV.Elements.ImagePreview,
                    className: "ddv-capture-viewer-image-preview-desktop",
                },
            ],
        };


        mainViewer?.hide();
        const captureViewer = new Dynamsoft.DDV.CaptureViewer({
            container: "container",
            uiConfig: pcCaptureUiConfig
        });
        captureViewer.openDocument(mainViewer?.currentDocument.uid); // Open a document which has pages
        captureViewer.play();
        captureViewer.on("close", () => {
            captureViewer.destroy();
            mainViewer?.show();
        });
    }

    static async createView(options) {
        this.productkey = options.productKey;
        this.messageType = options.messageType;

        // Public trial license which is valid for 24 hours
        // You can request a 30-day trial key from https://www.dynamsoft.com/customer/license/trialLicense/?product=mwc
        Dynamsoft.DDV.Core.license = this.productkey;
        // Preload DDV Resource
        Dynamsoft.DDV.Core.loadWasm();
        await Dynamsoft.DDV.Core.init();
        Dynamsoft.DDV.setProcessingHandler("imageFilter", new Dynamsoft.DDV.ImageFilter());

        if (options.uiConfig === 'desktop-default') {
            this.myViewer = new MyDesktopViewer(options);
        }else if (options.uiConfig === 'mobile-default') {
            this.myViewer = new MyMobileViewer(options);
        }
        window["myViewer"] = this.myViewer;

        return location.origin;
    }
}

window.addEventListener("load", function () {
    MyViewerApp.messageType = '__RawMessage'; // hybridwebview require this
    MyViewerApp.invokeDotNet('load', 'true', '');
    MyViewerApp.messageType = ''; // reset this, after initView, we will set it as real type
});

async function invokeJavaScript(name, params, context) {
    return MyViewerApp.invokeJavaScript(name, params, context);
}


