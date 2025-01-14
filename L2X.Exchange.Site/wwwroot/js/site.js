(function (factory) {
	if (typeof define === 'function' && define.amd) {
		define(['jquery'], factory);
	} else if (typeof exports === 'object') {
		factory(require('jquery'));
	} else {
		factory(jQuery);
	}
}(function ($) {
	var pluses = /\+/g;
	function encode(s) {
		return config.raw ? s : encodeURIComponent(s);
	}
	function decode(s) {
		return config.raw ? s : decodeURIComponent(s);
	}
	function stringifyCookieValue(value) {
		return encode(config.json ? JSON.stringify(value) : String(value));
	}
	function parseCookieValue(s) {
		if (s.indexOf('"') === 0) {
			// This is a quoted cookie as according to RFC2068, unescape...
			s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\');
		}

		try {
			// Replace server-side written pluses with spaces.
			// If we can't decode the cookie, ignore it, it's unusable.
			// If we can't parse the cookie, ignore it, it's unusable.
			s = decodeURIComponent(s.replace(pluses, ' '));
			return config.json ? JSON.parse(s) : s;
		} catch (e) { }
	}
	function read(s, converter) {
		var value = config.raw ? s : parseCookieValue(s);
		return $.isFunction(converter) ? converter(value) : value;
	}
	var config = $.cookie = function (key, value, options) {
		// Write
		if (value !== undefined && !$.isFunction(value)) {
			options = $.extend({}, config.defaults, options);

			if (typeof options.expires === 'number') {
				var days = options.expires, t = options.expires = new Date();
				t.setTime(+t + days * 864e+5);
			}

			return (document.cookie = [
				encode(key), '=', stringifyCookieValue(value),
				options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
				options.path ? '; path=' + options.path : '',
				options.domain ? '; domain=' + options.domain : '',
				options.secure ? '; secure' : ''
			].join(''));
		}

		// Read
		var result = key ? undefined : {};

		// To prevent the for loop in the first place assign an empty array
		// in case there are no cookies at all. Also prevents odd result when
		// calling $.cookie().
		var cookies = document.cookie ? document.cookie.split('; ') : [];
		for (var i = 0, l = cookies.length; i < l; i++) {
			var parts = cookies[i].split('=');
			var name = decode(parts.shift());
			var cookie = parts.join('=');

			if (key && key === name) {
				// If second argument (value) is a function it's a converter...
				result = read(cookie, value);
				break;
			}
			if (!key && (cookie = read(cookie)) !== undefined) {
				result[name] = cookie;
			}
		}

		return result;
	};

	config.defaults = {};
	$.removeCookie = function (key, options) {
		if ($.cookie(key) === undefined) {
			return false;
		}
		$.cookie(key, '', $.extend({}, options, { expires: -1 }));
		return !$.cookie(key);
	};
}));

var _api_service = 'https://localhost:7141/api/';
function $N(num) {
	if (num > 9) return num.toString();
	return `0${num.toString()}`;
}
function $P(url) {
	if (!url) return _api_service;

	url = url.toString();
	if (url.indexOf('${') >= 0) {
		for (var v in _request) {
			var rx = new RegExp(`\\$\\{${v}\\}`, 'gi');
			url = url.replace(rx, _request[v]);
		}
	}

	if (url.indexOf('#') == 0) return url;
	if (url.indexOf('~/') == 0) {
		return `${_api_service}${url.substring(2)}`;
	}
	if (url.indexOf('/') == 0) {
		return `${_api_service}${url.substring(1)}`;
	}
	return `${_api_service}${url}`;
}
function $X(num, fmt, unt) {
	if (num == 0) return fmt ? `<span class="deci">0<i>.</i><small>00</small>${unt ? '<u>' + unt + '</u>' : ''}</span>` : 0;
	if (!num || typeof num != 'number') return num;
	var res = '', unz = num < 0;
	var spl = num.toString().split(/[eE]/);
	if (spl.length == 1) {
		if (!spl[0].length) return num;
		res = unz ? spl[0].substring(1) : spl[0];
	}
	else if (spl.length == 2) {
		if (!spl[1].length) return num;

		var ss = spl[0].split('.'), n1 = parseInt(spl[1]), fxNo = '';
		if (ss.length == 1) {
			fxNo = ss[0];
		}
		else {
			fxNo = `${ss[0]}${ss[1]}`;
			n1 -= ss[1].length;
		}

		if (unz) fxNo = fxNo.substring(1);

		var l1 = fxNo.length;
		if (n1 < 0) {
			n1 = -n1;
			if (n1 >= l1) {
				res = '0.';
				for (var i = 0; i < n1 - l1; i++) {
					res += '0'
				}
				res += fxNo;
			}
			else {
				res = res.substring(0, l1 - n1) + ',' + res.substring(l1 - n1);
			}
		}
		else {
			res = fxNo;
			for (var i = 0; i < n1; i++) {
				res += '0'
			}
		}
	}
	else return num;
	if (!fmt) return `${unz ? '-' : ''}${res}`;

	spl = res.split('.');
	res = spl.length == 1 ? '' : `<i>,</i><small>${spl[1]}</small>`;
	var fxNo = spl[0], l1 = fxNo.length;
	for (var i = 0; i < l1; i++) {
		res = fxNo[l1 - i - 1] + res;
		if (i % 3 == 2 && i < l1 - 1) res = '<i>.</i>' + res;
	}
	return `<span class="deci">${unz ? '-' : ''}${res}${unt ? '<u>' + unt + '</u>' : ''}</span>`;
}
function $D(date) {
	if (!date) return '';

	try {
		var d = date;
		if (!(Object.prototype.toString.call(date) === '[object Date]' && !isNaN(date))) {
			d = new Date(date);
		}
		return `${$N(d.getHours())}:${$N(d.getMinutes())} ${$N(d.getDate())}/${$N(d.getMonth() + 1)}/${d.getFullYear()}`;
	}
	catch { return ''; }
}

function raiseError(xhr) {
	if (xhr.status == '401' || xhr.status == '403') {
		_notify.pushAlert('Not Authorized!');
		return;
	}
	if (xhr.status == '404') {
		_notify.pushAlert('Entity Not Found!');
		return;
	}

	var json = xhr.responseJSON;
	if (json && json.error) {
		_notify.pushError(json.message || json.error || 'Unknown error');
	}
	else if (json && json.errors) {
		var s = '';
		for (var v in json.errors) {
			s += json.errors[v] + ';';
		}
		_notify.pushError(s || 'Unknown error');
	}
	else {
		_notify.pushError('Service Unavailable');
	}
	return [];
}

function TableData(element) {
    var _elem = $(element);
    var _path = _elem.data('action');
	var _method = (_elem.data('method') || 'GET').toUpperCase();
	var _refresh = parseInt(_elem.data('refresh') || "0");
    var _config = {
        ajax: {},
        responsive: true,
        ordering: false,
        processing: true,
        info: !(_elem.data('paging') == 'no'),
        paging: !(_elem.data('paging') == 'no'),
        bPaginate: !(_elem.data('paging') == 'no'),
        serverSide: true,
        lengthMenu: [50, 100, 500],
        pageLength: 50,
        columns: [],
        select: { style: 'single' },
        destroy: true,
    };

    $('thead th', _elem).each(function () {
        var d = $(this).data('ref'), c = { data: d };
        switch (d) {
            case 'id':
            case 'idkey': c.className = 'key'; break;

            case 'name':
            case 'owner':
            case 'title':
            case 'caption': c.className = 'name'; break;

            case 'createdat':
			case 'expiredat':
			case 'finishedat':
            case 'lastlogin':
            case 'lastsent':
            case 'lockeduntil':
            case 'regdate': c.className = 'date';
                c.render = function (data, type, full, meta) {
                    return $D(data);
                };
                break;

            case 'state':
            case 'type':
                c.render = function (data, type, full, meta) {
                    return `<span class="sts ${data}">${data}</span>`;
                };
                break;

            case 'side':
            case 'taker_type':
                c.render = function (data, type, full, meta) {
                    return data == 'buy' ? `<span class="sts upp">Buy</span>` : `<span class="sts dwn">Sell</span>`;
                };
                break;

            default:
                c.render = function (data, type, full, meta) {
                    if (data && typeof data == 'object') {
                        return c.name ?? c.code ?? c.idkey ?? '';
                    }
                    return $X(data, 1) ?? '';
                };
                break;
        }
        _config.columns.push(c);
	});

    if (_method == 'POST') {
        _config.ajax = {
            type: 'POST',
            url: $P(_path),
            //beforeSend: function (req) {
            //    req.setRequestHeader("Authorization", $.cookie('token') || '');
            //},
            contentType: "application/json",
            dataType: 'json',
			data: function (d) {
                if (_config.paging) return JSON.stringify({
                    page: (parseInt(d.start / d.length) || 0) + 1,
                    size: d.length,
                    search: d.search.value,
                });
                else return JSON.stringify({ search: d.search.value, });
            },
            dataSrc: _config.paging ? {
                draw: 1,
                data: "data",
                recordsTotal: "total_items",
                recordsFiltered: "total_items",
                error: "message"
            } : {
                draw: 1,
                data: "data",
                error: "message"
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        };
    }
    else {
        _config.paging = false;
        _config.info = false;
        _config.searching = false;
        _config.ajax = {
            url: $P(_path),
            type: 'GET',
            //beforeSend: function (req) {
            //    req.setRequestHeader("Authorization", $.cookie('token') || '');
            //},
			data: function (d) {
                return {
                    page: (parseInt(d.start / d.length) || 0) + 1,
                    size: d.length && d.length > 0 ? d.length : 15
                };
            },
			dataSrc: function (d) {
				return d.data instanceof Array ? d.data : (d.data ? [d.data] : []);
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        };
    }

    var dtbl = _elem.DataTable(_config);
    dtbl.elem = _elem;
    dtbl.urlpath = _path;
    dtbl.refresh = function () {
        if (!_init) {
            _init = true;
            return;
        }
        _total = 0;
        _availed = 0;
        _locked = 0;
        _setted = 0;
        if (_path.indexOf("${") >= 0) {
            dtbl.ajax.url($P(_path)).load();
        }
        else {
            dtbl.ajax.reload();
        }
    };

    dtbl.isElm = function (e) {
        return dtbl.elem && dtbl.elem.attr('id') == e.substring(1);
    };
    dtbl.selections = function () {
        var ids = [];
        $('input[type=checkbox]', dtbl.elem).each(function () {
            var a = this;
            if (a.checked && a.name == 'sel') ids.push(a.value);
        });
        return ids;
	};

	if (_refresh) {
		dtbl.timing = 0;
		dtbl.retry = function () {
			if (dtbl.timing) {
				clearTimeout(dtbl.timing);
				dtbl.timing = 0;
			}
			dtbl.timing = setTimeout(function () { dtbl.ajax.reload(); }, _refresh);
		};
		dtbl.retry();
	}
    return dtbl;
}

$(document).ready(function () {
	//_body = $('body');
	//var _isAdmin = _body.hasClass('admin');
	//var _isAuth = ($.cookie('token') || '').indexOf('Bearer ') > -1;
	//if (_isAuth) {
	//	_body.addClass('authed');
	//}
	//else {
	//	if (_body.hasClass('must-auth')) {
	//		window.location = 'auth.html';
	//	}
	//}

	_navigate = $('.sidenav') || {};
	_navigate.change = function (e) {
		if (!_navigate.length) return;
		e = e.indexOf('#') == 0 ? e : `#${e}`;
		$('a', _navigate).each(function () {
			var a = $(this);
			if (a.attr('href') == e) {
				if (!a.hasClass('active')) { a.addClass('active'); }
			}
			else {
				a.removeClass('active');
			}
			if ($(window).width() < 1201) {
				var b = $('body');
				if (b.hasClass('has-side')) b.removeClass('has-side');
			}
		});
	};
	if (_navigate.length) {
		$('.menuer').click(function (e) {
			e.preventDefault();
			$('body').toggleClass('has-side');
		});

		if ($(window).width() > 1200) { $('.menuer').click(); }
	}
	$('.darker').click(function (e) {
		e.preventDefault();
		$('body').toggleClass('darke');
	});
	$('.logout').click(function (e) {
		logout();
	});

	var _noti_show = 0;
	_notify = $('.sidebar.noti-list') || {};
	_notify.counter = $('.notifier small');
	_notify.tipter = $('.msgtip');
	_notify.show = function (e) {
		if (!e) {
			_notify.removeClass('active');
			_notify.setCounter(0);
		}
		else if (!_notify.hasClass('active')) _notify.addClass('active');
	};
	_notify.setCounter = function (e) {
		_notify.counter.html(e ? e.toString() : '');
	};
	_notify.addCounter = function (e) {
		var a = parseInt(_notify.counter.html() || 0) + e;
		_notify.counter.html(e ? e.toString() : '');
	};
	_notify.pushMsg = function (msg, tmr, typ) {
		if (_noti_show) {
			clearTimeout(_noti_show);
			_noti_show = 0;
		}
		_noti_show = setTimeout(function () {
			$('.msg-cont', _notify.tipter).html(msg);
			if (_notify.tipter.hasClass('hide')) {
				_notify.tipter.attr('class', `msgtip${typ ? ' ' + typ : ''}`);

				if (_noti_show) {
					clearTimeout(_noti_show);
					_noti_show = 0;
				}
				_noti_show = setTimeout(function () {
					if (!_notify.tipter.hasClass('hide')) _notify.tipter.addClass('hide');
				}, tmr || 5000);
			}
		}, 10);
	};
	_notify.pushAlert = function (msg) {
		_notify.pushMsg(msg, 3000, 'alert');
	};
	_notify.pushError = function (msg) {
		_notify.pushMsg(msg, 5000, 'error');
	};
	_notify.pushOk = function (msg) {
		_notify.pushMsg(msg, 3000, 'succ');
	};
	$('.notifier').click(function (e) {
		e.preventDefault();
		var a = $(this).toggleClass('active');
		_notify.show(a.hasClass('active'));
	});
	_notify.tipter.click(function (e) {
		e.preventDefault();
		if (_noti_show) {
			clearTimeout(_noti_show);
			_noti_show = 0;
		}
		_notify.tipter.attr('class', `msgtip hide`);
	});

	$('.dtable').each(function () {
		var dtbl = TableData(this);
	});

	//if (_body.hasClass('xchange')) {
	//	XchangeView(_body);
	//}
	//else {
	//	_module = 0;
	//	var _module_default = '';
	//	$('section').each(function () {
	//		var s = $(this), id = s.attr('id');
	//		if (id && !s.hasClass('hide')) {
	//			s.addClass('hide');
	//			_module_default = `#${id}`;
	//		}
	//		return false;
	//	});

	//	//var _module_changing = 0;
	//	//function moduleChange() {
	//	//	var m = (location.hash || _module_default);
	//	//	var mdl = ModuleView(m);
	//	//	if (_module == mdl) return;
	//	//	if (_module_changing) {
	//	//		clearTimeout(_module_changing);
	//	//		_module_changing = 0;
	//	//	}

	//	//	_module_changing = setTimeout(function () {
	//	//		if (_module) { _module.show(0); }
	//	//		_module = mdl;
	//	//		_module.show(1);
	//	//		_navigate.change(m);
	//	//	}, 100);
	//	//}
	//	//$(window).on('hashchange', function () {
	//	//	moduleChange();
	//	//});

	//	//window.refresh = function () {
	//	//	moduleChange();
	//	//};
	//	//moduleChange();
	//}
});