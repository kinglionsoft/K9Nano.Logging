// 获取url参数
const getQuery = url => {
  let result = {};
  url.replace(/([^?&=]+)=([^&]+)/g, ($0, $1, $2) => result[$1] = $2);
  return result;
}

// 解析对象的url
const decodeURIObj = obj => {
  let result = JSON.parse(JSON.stringify(obj));
  for (const key in result) {
    if (result.hasOwnProperty(key)) {
      result[key] = JSON.parse(decodeURI(result[key]));
    }
  }
  return result;
}

// 解析对象转成url的search参数
const objToSearchString = obj => {
  let str = `?`;
  for (const key in obj) {
    if (obj.hasOwnProperty(key)) {
      str += `${key}=${JSON.stringify(obj[key])}&`;
    }
  }
  return str.replace(/&$/, "");
}

export {
  getQuery,
  decodeURIObj,
  objToSearchString
}