// 除了類別名稱"Plusone"，此檔案中的所有命名皆需為駝峰命名法，這樣比較好記。

(function (root) {

  /**
 * Author: Plusone
 * Email: plusone@creatidea.com.tw
 */
  class Plusone {

    /**
     * 檢查物件是否已經定義
     * @param {any} obj 受檢物件
     * @return {boolean} 物件是否已經定義
     */
    static isDefinded(obj) {
      return obj !== void 0;
    }

    /**
     * 檢查物件是否為字串
     * @param {any} obj 受檢物件
     * @return {boolean} 物件是否為字串
     */
    static isString(obj) {
      return typeof obj == "string";
    }

    /**
     * 檢查物件是否為布林
     * @param {any} obj 受檢物件
     * @return {boolean} 物件是否為布林
     */
    static isBoolean(obj) {
      return typeof obj == "boolean";
    }

    /**
     * 檢查物件是否為數字
     * @param {any} obj 受檢物件
     * @return {boolean} 物件是否為數字
     */
    static isNumber(obj) {
      return typeof obj == "number";
    }

    /**
     * 檢查物件是否為function(函式)
     * @param {any} obj 受檢物件
     * @return {boolean} 物件是否為數字
     */
    static isFunction(obj) {
      return typeof obj === "function" && typeof obj.nodeType !== "number";
    }

    /**
     * 取得兩個時間的秒數差
     * @param {Date} date1 時間1
     * @param {Date} date2 時間2
     * @return {number} 時間2-時間1的秒數
     */
    static dateDiffInSecond(date1, date2) {
      const utc1 = Date.UTC(date1.getFullYear(),
        date1.getMonth(),
        date1.getDate(),
        date1.getHours(),
        date1.getMinutes(),
        date1.getSeconds());

      const utc2 = Date.UTC(date2.getFullYear(),
        date2.getMonth(),
        date2.getDate(),
        date2.getHours(),
        date2.getMinutes(),
        date2.getSeconds());

      return Math.floor((utc2 - utc1) / (1000));
    }

    /**
     * 檢查字串是否為Null或空白，將一併檢查輸入變數是否未定義
     * @param {string} str
     */
    static isNullOrWhitespace(str) {
      return str === void 0 || str === null || str.match(/^ *$/) !== null;
    }

    /**
     * 檢查字串是否為html顏色格式(#rrggbb，不分大小寫)
     * @param {any} str 要檢查的字串
     */
    static isHtmlColor(str) {
      return typeof str == "string" && str.match(/^#[0-9a-fA-F]{6}$/);
    }

    /**
     * 檢查字串是否為完整Url格式
     * ※ 這個函式未經過嚴謹的驗證
     * @param {any} str 要檢查的字串
     */
    static isValidUrl(str) {
      return typeof str == "string" && str.match(/^(http|https):\/\/+[\www\d]+\.[\w]+(\/[\w\d]+)?/);
    }

    /**
     * 檢查物件是不是DOM節點
     * @param {any} obj 受檢物件
     */
    static isNode(obj) {
      return typeof Node === "object"
        ? obj instanceof Node
        : obj && typeof obj === "object" && typeof obj.nodeType === "number" && typeof obj.nodeName === "string";
    }

    /**
     * 檢查物件是否為DOM元素
     * @param {any} obj 受檢物件
     */
    static isElement(obj) {
      return typeof HTMLElement === "object"
        ? obj instanceof HTMLElement
        : obj && typeof obj === "object" && obj.nodeType === 1 && typeof obj.nodeName === "string";
    }

    /**
     * 將RGB陣列轉為html顏色格式字串
     * Ex: [115, 132, 132] => #738484
     * @param {Array<number>} rgb RGB陣列
     * @return {string} html顏色格式字串
     */
    static rgb2HtmlColor(rgb) {
      if (!Array.isArray(rgb)) {
        throw `Argument type error. The accept type is array, not type: '${
        typeof rgb
        }'`;
      }

      if (rgb.length !== 3) {
        throw `Argument length error. The accept length is 3, not '${
        rgb.length
        }'`;
      }

      return `#${
        rgb[0].toString(16)
        }${
        rgb[1].toString(16)
        }${
        rgb[2].toString(16)
        }`;
    }

    /**
     * 將html顏色格式字串轉為RGB陣列
     * Ex: #738484 => [115, 132, 132]
     * @param {string} htmlColor html顏色格式字串
     * @return {Array<number>} 陣列
     */
    static htmlColor2Rgb(htmlColor) {
      if (!Plusone.isString(htmlColor)) {
        throw `Argument type error. The accept type is String, not type: '${
        typeof htmlColor
        }'`;
      }

      if (htmlColor.length !== 7) {
        throw `Argument length error. The accept length is 7, not '${
        htmlColor.length
        }'`;
      }

      const r = parseInt(htmlColor.substr(1, 2), 16);
      const g = parseInt(htmlColor.substr(3, 2), 16);
      const b = parseInt(htmlColor.substr(5, 2), 16);

      return [r, g, b];
    }

    /**
     * 取得一組新的Guid格式的字串
     * @return {string} 符合Guid格式的字串
     */
    static newGuid() {
      const s4 = () => Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
      return `${s4() + s4()}-${s4()}-${s4()}-${s4()}-${s4() + s4() + s4()}`;
    }
  }

  if (typeof exports === "object") {
    module.exports = Plusone;
  } else if (typeof define === "function" && define.amd) {
    define([], Plusone);
  } else {
    root.Plusone = Plusone;
  }
}(this));
