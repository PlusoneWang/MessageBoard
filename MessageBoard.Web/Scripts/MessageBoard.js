var messageBoard = new Vue({
    el: "#app",
    data: {
        currentMessage: {
            message: "",
            images: [],
            previewImages: []
        }, // 使用者當前輸入的留言
        messages: [], // 留言列表
        userId: userId, // 當前的使用者
        serverRoot: serverRoot,

        // 當前編輯物件
        currentEdit: {
            messageId: null, // 訊息Id
            context: "", // 訊息內容
            imageDelete: [], // 要刪除的圖片
            imageNew: [], // 要新增的圖片
            currentImages: [] // 顯示中的圖片(包含預覽圖)
        }
    },

    computed: {
        // 是否要顯示留言框的圖片群組
        showImageRow() {
            return Array.isArray(this.currentMessage.images) && this.currentMessage.images.length > 0;
        },

        // 是否顯顯示訊息列表
        showMessageList() {
            return this.messages.length > 0;
        },

        // 是否可以送出訊息
        messageSendable() {
            return this.currentMessage.message.length > 0 || this.showImageRow;
        },

        // 當前已顯示的訊息的Id陣列
        messageIds() {
            return this.messages.map(o => o.MessageId);
        },

        showEditMessageImageRow() {
            return this.currentEdit.currentImages.length > 0;
        },

        // 是否可儲存編輯
        editSaveable() {
            return this.currentEdit.context.length > 0 || this.currentEdit.currentImages.length > 0;
        }
    },

    watch: {
        // 這是多餘的驗證，除非textarea的maxlength屬性失效，否則不會觸發這個alert
        "currentMessage.message": function (newMessage, oldMessage) {
            if (newMessage.length > 300) {
                swal("留言字數上限為300字");
                this.currentMessage.message = oldMessage;
            }
        }
    },

    created() {
        // get messages
        axios.post(Router.action("Home", "GetMessageList"))
            .then(function (response) {
                const data = response.data;
                if (data.Success === true) {
                    this.messages = this.messages.concat(data.Data);
                    return;
                }
                swal(`${data.Message}`);
            }.bind(this))
            .catch(function (error) {
                if (error.response) {
                    console.log(error.response.data);
                    console.log(error.response.status);
                    console.log(error.response.headers);
                } else if (error.request) {
                    console.log(error.request);
                } else {
                    console.log('Error', error.message);
                }
                console.log(error.config);
            }.bind(this));

        // 捲動更新
        $(window).scroll(this.tryLoadMoreMessages);
        
        // signalr
        var messageHub = $.connection.messageHub;
        messageHub.client.updateMessage = this.updateMessage;
        $.connection.hub.start();
    },

    methods: {
        // 載入預覽圖
        loadPreviewImage(event) {
            const fileInput = event.target;
            if (fileInput.files && fileInput.files.length > 0) {
                for (const file of fileInput.files) {
                    const fileSizeInMb = file.size / 1024 / 1024;
                    if (fileSizeInMb > 1) {
                        swal(`檔案${
                            escape(file.name)
                            }的大小超出限制\n允許的檔案大小為: 1 MB\n該檔案的大小為: ${
                            fileSizeInMb.toFixed(4)
                            } MB`);
                        fileInput.value = "";
                        return;
                    }
                }

                for (const file of fileInput.files) {
                    this.currentMessage.images.push(file);
                    const fileReader = new FileReader();
                    fileReader.onload = function (e) {
                        this.currentMessage.previewImages.push(e.target.result);
                    }.bind(this);
                    fileReader.readAsDataURL(file);
                }

                fileInput.value = "";
            }
        },

        // 取得新留言
        tryLoadMoreMessages() {
            if (!Utils.isElementInView($('#dataFlag'), false)) return;

            $(window).unbind('scroll', this.tryLoadMoreMessages);

            const excludeMessages = this.messageIds;
            axios.post(Router.action("Home", "GetMessageList"), excludeMessages)
                .then(function (response) {
                    const data = response.data;
                    if (data.Success === true) {
                        this.messages = this.messages.concat(data.Data);
                        if (data.Data.length === 0) {
                            $("#dataFlag").remove();
                        } else {
                            $(window).scroll(this.tryLoadMoreMessages);
                        }
                        return;
                    }
                    swal(`${data.Message}`);
                }.bind(this))
                .catch(function (error) {
                    if (error.response) {
                        console.log(error.response.data);
                        console.log(error.response.status);
                        console.log(error.response.headers);
                    } else if (error.request) {
                        console.log(error.request);
                    } else {
                        console.log('Error', error.message);
                    }
                    console.log(error.config);
                }.bind(this));
        },

        // 刪除預覽圖
        removePreviewImage(index) {
            this.currentMessage.images.splice(index, 1);
            this.currentMessage.previewImages.splice(index, 1);
        },

        // 取得頭像url
        getHeadPortraitUrl(userId) {
            return Router.action('Account', 'HeadPortrait', { id: userId });
        },

        // 留言
        sendMessage() {
            if (this.messageSendable !== true) {
                swal("沒有可以留言的內容，請重新進入頁面後再試一次。");
                return false;
            }

            const formData = new FormData();
            let index = 0;
            for (const image of this.currentMessage.images) {
                formData.append(`images[${
                    index
                    }]`,
                    image,
                    image.name);
                index++;
            }

            formData.append("messageContext", this.currentMessage.message);

            const config = {
                headers: { 'content-type': 'multipart/form-data' }
            }
            axios.post(Router.action("Home", "SendMessage"), formData, config)
                .then(function (response) {
                    const data = response.data;
                    if (data.Success !== true) {
                        swal(`${
                            data.Message
                            }`);
                    }
                    this.currentMessage.images = [];
                    this.currentMessage.previewImages.length = 0;
                    this.currentMessage.message = "";
                }.bind(this))
                .catch(function (error) {
                    if (error.response) {
                        console.log(error.response.data);
                        console.log(error.response.status);
                        console.log(error.response.headers);
                    } else if (error.request) {
                        console.log(error.request);
                    } else {
                        console.log('Error', error.message);
                    }
                    console.log(error.config);
                }.bind(this));
        },

        // 編輯訊息(初始化)
        onEdit(message) {
            this.currentEdit.messageId = message.MessageId;
            this.currentEdit.context = message.Context;
            this.currentEdit.currentImages = JSON.parse(JSON.stringify(message.AttachmentList));

            for (const image of this.currentEdit.currentImages) {
                image.srcValue = this.resolveUrl(image.ImagePath);
            }

            $("#editMessageModal").modal("show");
        },

        // 編輯功能，載入預覽圖
        loadPreviewEditMesageImage(event) {
            const fileInput = event.target;
            if (fileInput.files && fileInput.files.length > 0) {
                for (const file of fileInput.files) {
                    const fileSizeInMb = file.size / 1024 / 1024;
                    if (fileSizeInMb > 1) {
                        swal(`檔案${
                            escape(file.name)
                            }的大小超出限制\n允許的檔案大小為: 1 MB\n該檔案的大小為: ${
                            fileSizeInMb.toFixed(4)
                            } MB`);
                        fileInput.value = "";
                        return;
                    }
                }

                for (const file of fileInput.files) {
                    const localId = Plusone.newGuid();
                    this.currentEdit.imageNew.push({ localId, file });
                    const fileReader = new FileReader();
                    fileReader.onload = function (e) {
                        this.currentEdit.currentImages.push({ ImageId: null, srcValue: e.target.result, localId });
                    }.bind(this);;
                    fileReader.readAsDataURL(file);
                }

                fileInput.value = "";
            }
        },

        // 移除編輯中的圖片
        removeCurrentEditImage(image) {
            const indexOfCurrent = this.currentEdit.currentImages.indexOf(image);
            if (indexOfCurrent === -1) return;
            if (image.ImageId !== null) {
                this.currentEdit.imageDelete.push(image.ImageId);
            } else {
                const indexOfNew = this.currentEdit.imageNew.findIndex((ele) => ele.localId === image.localId);
                if (indexOfNew !== -1)
                    this.currentEdit.imageNew.splice(indexOfNew, 1);
            }

            this.currentEdit.currentImages.splice(indexOfCurrent, 1);
            return;
        },

        // 儲存編輯
        saveEdit() {
            if (this.editSaveable !== true) {
                swal("沒有可以儲存的內容，請再試一次。");
                return false;
            }

            const formData = new FormData();
            let index = 0;
            for (const image of this.currentEdit.imageNew) {
                formData.append(`images[${
                    index
                    }]`,
                    image.file,
                    image.file.name);
                index++;
            }

            formData.append("messageId", this.currentEdit.messageId);
            formData.append("context", this.currentEdit.context);
            formData.append("deleteImages", this.currentEdit.imageDelete);

            const config = {
                headers: { 'content-type': 'multipart/form-data' }
            }
            axios.post(Router.action("Home", "UpdateMessage"), formData, config)
                .then(function (response) {
                    $("#editMessageModal").modal("hide");
                    const data = response.data;
                    if (data.Success !== true) {
                        swal(`${
                            data.Message
                            }`);
                    }

                }.bind(this))
                .catch(function (error) {
                    if (error.response) {
                        console.log(error.response.data);
                        console.log(error.response.status);
                        console.log(error.response.headers);
                    } else if (error.request) {
                        console.log(error.request);
                    } else {
                        console.log('Error', error.message);
                    }
                    console.log(error.config);
                }.bind(this));
        },

        // 刪除留言
        deleteMessage(messageId) {
            swal({
                title: "這是不可逆的操作，確定嗎",
                type: "question",
                showCancelButton: true,
                confirmButtonColor: "#007bff",
                confirmButtonText: "確定",
                cancelButtonText: "取消"
            }).then(function (result) {
                if (result.value) {
                    axios.post(Router.action("Home", "DeleteMessage"), { messageId })
                        .then(function (response) {
                            const data = response.data;
                            if (data.Success !== true) {
                                if (data.Message) {
                                    swal(data.Message);
                                }
                            }
                        }.bind(this))
                        .catch(function (error) {
                            if (error.response) {
                                console.log(error.response.data);
                                console.log(error.response.status);
                                console.log(error.response.headers);
                            } else if (error.request) {
                                console.log(error.request);
                            } else {
                                console.log('Error', error.message);
                            }
                            console.log(error.config);
                        }.bind(this));
                }
            }.bind(this));
        },

        /**
         * 更新訊息
         * @param {any} targetId 目標訊息Id
         * @param {any} updateType 動作類型
         * @param {any} data 新資料
         */
        updateMessage(targetId, updateType, data) {
            switch (updateType) {
                case "newMessage":
                    {
                        // insert new message
                        this.messages.splice(0, 0, data);
                        break;
                    }
                case "newReply":
                    {
                        // insert new reply to target message
                        const message = this.messages.find((element) => { return element.MessageId === targetId });
                        if (message) {
                            message.ReplyMessages.splice(0, 0, data);
                        }
                        break;
                    }
                case "updateMessage":
                    {
                        // replace target message
                        for (let i = 0; i < this.messages.length; i++) {
                            if (this.messages[i].MessageId === targetId) {
                                const replyMessages = JSON.parse(JSON.stringify(this.messages[i].ReplyMessages));
                                data.ReplyMessages = replyMessages;
                                this.messages.splice(i, 1, data);
                                break;
                            } else if (this.messages[i].ReplyMessages.length > 0) {
                                for (let j = 0; j < this.messages[i].ReplyMessages.length; j++) {
                                    if (this.messages[i].ReplyMessages[j].MessageId === targetId) {
                                        this.messages[i].ReplyMessages[j].splice(j, 1, data);
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case "delete":
                    {
                        // remove target message
                        for (let i = 0; i < this.messages.length; i++) {
                            if (this.messages[i].MessageId === targetId) {
                                this.messages.splice(i, 1);
                                break;
                            } else if (this.messages[i].ReplyMessages.length > 0) {
                                for (let j = 0; j < this.messages[i].ReplyMessages.length; j++) {
                                    if (this.messages[i].ReplyMessages[j].MessageId === targetId) {
                                        this.messages[i].ReplyMessages[j].splice(j, 1);
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        console.warn("未預期的動作類型");
                    }
            }
        },

        // 產出image路徑
        resolveUrl(originalUrl) {
            return this.serverRoot + originalUrl.substring(2);
        },

        // 是否可刪除
        deleteable(messageOwnerId) {
            return (messageOwnerId === this.userId) || (typeof isAdmin != typeof void 0 && isAdmin === true);
        },

        // 是否可編輯
        editable(messageOwnerId) {
            return messageOwnerId === this.userId;
        }
    }
});


$(function() {
    // 編輯視窗隱藏
    $("#editMessageModal").on("hidden.bs.modal",  function (e) {
        messageBoard.currentEdit.messageId = null;
        messageBoard.currentEdit.context = "";
        messageBoard.currentEdit.imageDelete = [];
        messageBoard.currentEdit.imageNew = [];
        messageBoard.currentEdit.currentImages = [];
    });
})