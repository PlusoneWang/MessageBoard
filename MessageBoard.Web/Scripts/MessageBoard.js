var messageBoard = new Vue({
    el: "#app",
    data: {
        currentMessage: {
            message: "",
            images: [],
            previewImages: []
        }, // 使用者當前輸入的留言
        messages: [], // 留言列表
        userId: window.userId
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
        this.loadMoreMessages();
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
                        swal(`檔案${escape(file.name)}的大小超出限制\n允許的檔案大小為: 1 MB\n該檔案的大小為: ${fileSizeInMb.toFixed(4)} MB`);
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
        loadMoreMessages() {
            const excludeMessages = this.messageIds;
            axios.get(Router.action("Home", "GetMessageList"), excludeMessages)
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
                formData.append(`images[${index}]`, image, image.name);
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
                        swal(`${data.Message}`);
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

        deleteMessage() {
            // TODO delete message
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
                            }
                            else if (this.messages[i].ReplyMessages.length > 0) {
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
                            }
                            else if (this.messages[i].ReplyMessages.length > 0) {
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

        authorizeUser(messageOwnerId) {
            return messageOwnerId === this.userId;
        }
    }
})