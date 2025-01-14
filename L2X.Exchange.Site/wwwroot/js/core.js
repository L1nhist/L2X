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
		} catch(e) {}
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
				options.path    ? '; path=' + options.path : '',
				options.domain  ? '; domain=' + options.domain : '',
				options.secure  ? '; secure' : ''
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

var _request = {};
var _module_lists = {};
var _api_service = 'https://localhost:7099/api/v1/';
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
    if (xhr.status == '401' || xhr.status == '403')
    {
        _notify.pushAlert('Not Authorized!');
        return;
    }
    if (xhr.status == '404')
    {
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
function logout() {
    $.removeCookie('name');
    $.removeCookie('email');
    $.removeCookie('token');
    window.location = 'auth.html';
}
function Picker(element, opt) {
    var _pkr = $(element), _action = _pkr.data('action'), _empty = _pkr.data('empty') || 0;
    if (!_action) return;

    var _tgt = _pkr;
    var t = _pkr.data("target");
    if (t) {
        t = $(t);
        if (t.length) _tgt = t;
    }

    $.ajax({
        type: 'GET',
        url: $P(_action),
        contentType: 'application/json; charset=UTF-8',
        processData: false,
        headers: { "Authorization": $.cookie('token') || '' },
        success: function (json) {
            if (json.error) {
                _notify.pushAlert(json.message);
                return;
            }
            if (json.data && json.data.length) {
                var d = json.data;
                _tgt.empty();
                if (_empty) $(`<option value="" selected>${_empty}</option>`).appendTo(_tgt);
                for (var i = 0; i < d.length; i++) {
                    if (typeof(d[i]) == 'object') $(`<option value="${d[i].key}"${i == 0 && !_empty ? 'selected' : ''}>${d[i].name || d[i].key}</option>`).appendTo(_tgt);
                    else $(`<option value="${d[i]}"${i == 0 && !_empty ? 'selected' : ''}>${d[i]}</option>`).appendTo(_tgt);
                }
            }
        },
        error: function(xhr) {
            _tgt.empty();
            if (_empty) $(`<option value="" selected>${_empty}</option>`).appendTo(t);
        }
    });

    if (_pkr.attr('id') == 'mbeSel' || _pkr.attr('id') == 'tkeSel') {
        _pkr.change(function(e){
            var _sel = $('#wadSel'), _urls = _sel.data('action'), _empty = _pkr.data('empty') || 0;
            var mem = $('#mbeSel').val(),tkr = $('#tkeSel').val();
            if (!mem || !tkr) return;

            _urls = _urls.replace('${member}', mem).replace('${ticker}', tkr);

            $.ajax({
                type: 'GET',
                url: $P(_urls),
                contentType: 'application/json; charset=UTF-8',
                processData: false,
                headers: { "Authorization": $.cookie('token') || '' },
                success: function (json) {
                    if (json.error) {
                        _notify.pushAlert(json.message);
                        return;
                    }
                    if (json.data && json.data.length) {
                        var d = json.data;
                        _sel.empty();
                        if (_empty) $(`<option value="" selected>${_empty}</option>`).appendTo(_sel);
                        for (var i = 0; i < d.length; i++) {
                            if (typeof(d[i]) == 'object') $(`<option value="${d[i].key}"${i == 0 && !_empty ? 'selected' : ''}>${d[i].name || d[i].key}</option>`).appendTo(_sel);
                            else $(`<option value="${d[i]}"${i == 0 && !_empty ? 'selected' : ''}>${d[i]}</option>`).appendTo(_sel);
                        }
                    }
                },
                error: function(xhr) {
                    _sel.empty();
                    if (_empty) $(`<option value="" selected>${_empty}</option>`).appendTo(t);
                }
            });
        });
    }
    return _pkr;
}
function TableData(element, owner) {
    var _elem = $(element);
    var _owner = owner;
    var _init = false;
    var _total = 0, _availed = 0, _locked = 0, _setted = 0;
    var _path = _elem.data('action');
    if (_owner && typeof(_owner.path) == 'function') {
        _path = _owner.path(_path);
    }
    var _method = (_elem.data('method') || 'GET').toUpperCase();
    var _config = {
        ajax: {},
        responsive: true,
        ordering: false,
        processing: true,
        info: !(_elem.data('paging') == 'no'),
        paging: !(_elem.data('paging') == 'no'),
        bPaginate: !(_elem.data('paging') == 'no'),
        serverSide: true,
        lengthMenu: [15, 30, 50, 100],
        pageLength: 15,
        columns: [],
        select: { style: 'single' },
        destroy: true,
        initComplete: function() {
            if (_setted) {
                $('#totalAmount').html($X(_total, 1));
                $('#availedAmount').html($X(_availed, 1));
                $('#lockedAmount').html($X(_locked, 1));
            }
        }
    };

    var idx = -1;
    $('thead th', _elem).each(function(){
        var d = $(this).data('ref'), c = { data: d };
        switch (d)
        {
            case 'id':
            case 'idkey': c.className = 'key'; break;
            case 'checkee':
                c.className = 'chk';
                c.data = 'idkey';
                c.render = function (data, type, full, meta) {
                    return `<input type="checkbox" name="sel" value="${data}" />`;
                };
                break;
    
            case 'code':
                c.className = `code${_owner.isElm('#ticker') ? ' tkr' : ''}`; break;
    
            case 'ticker':
            case 'ticker.code':
            case 'instr':
            case 'instr.code':
            case 'maker_order.code':
            case 'taker_order.code':
                c.className = 'code tkr'; break;

            case 'name':
            case 'title':
            case 'caption': c.className = 'name'; break;
            
            case 'created_at':
            case 'last_changed':
            case 'last_login':
            case 'last_sent':
            case 'locked_until':
            case 'reg_date': c.className = 'date';
                c.render = function (data, type, full, meta) {
                    return $D(data);
                };
                break;
    
            case 'ticker_logo': c.className = 'logo'; c.data = 'ticker';
                c.render = function (data, type, full, meta) {
                    if (!data.logo || data.logo == '#') return '<i class="ico logo x2"></i>';
                    return `<i class="ico logo x2"><img src="${data.code.toUpperCase()}" /></i>`;
                };
                break;
    
            case 'ticker_name': c.data = 'ticker';
                c.render = function (data, type, full, meta) {
                    return `<h6>${data.code.toUpperCase()}</h6><small>${data.name}</small>`;
                };
                break;
    
            case 'balance': c.className = 'num';
                c.render = function (data, type, full, meta) {
                    var amt = parseFloat(data || 0);
                    var prc = parseFloat(full.ticker.price || 0);
                    _total += amt * prc;
                    _setted = 1;
                    return `<b>${$X(amt, 1)}</b>`;
                };
                break;
    
            case 'avail_amount': c.className = 'num';
                c.render = function (data, type, full, meta) {
                    var amt = parseFloat(data || 0);
                    var prc = parseFloat(full.ticker.price || 0);
                    _availed += amt * prc;
                    _setted = 1;
                    return $X(amt, 1);
                };
                break;
    
            case 'lock_amount': c.className = 'num';
                c.render = function (data, type, full, meta) {
                    var amt = parseFloat(data || 0);
                    var prc = parseFloat(full.ticker.price || 0);
                    _locked += amt * prc;
                    _setted = 1;
                    return $X(amt, 1);
                };
                break;
    
            case 'accounting':
                c.render = function (data, type, full, meta) {
                    var html = '';
                    if (full.ticker.can_deposit) {
                        html += `<a class="btn sm ask" href="#deposit?ticker=${full.ticker.code}">Nạp</a>`;
                    }
                    if (full.ticker.can_withdraw) {
                        html += `<a class="btn sm bid" href="#withdraw?ticker=${full.ticker.code}">Rút</a>`;
                    }
                    if (html == '') {
                        html += `<a class="btn sm" href="#account?ticker=${full.ticker.code}">Xem</a>`;
                    }
                    return `<div class="nowrap">${html}</div>`;
                };
                break;
    
            case 'state':
            case 'type':
                c.render = function (data, type, full, meta) {
                    return `<span class="sts ${data}">${data}</span>`;
                };
                break;
    
            case 'mch_order':
                c.className = 'code tkr';
                c.render = function (data, type, full, meta) {
                    var a = _request['order'], b = d.substring(4);
                    if (full['maker_order'].code == a)
                    {
                        b = 'taker_' + b;
                    }
                    else {
                        b = 'maker_' + b;
                    }
                    
                    return full[b].code;
                };
                break;

            case 'mch_owner':
                c.render = function (data, type, full, meta) {
                    var a = _request['order'], b = d.substring(4);
                    if (full['maker_order'].code == a)
                    {
                        b = 'taker';
                    }
                    else {
                        b = 'maker';
                    }
                    
                    return full[b].email;
                };
                break;

            case 'mch_type':
                c.render = function (data, type, full, meta) {
                    var a = _request['order'], b = d.substring(4), c = '';
                    if (full['maker_order'].code == a)
                    {
                        b = 'taker';
                    }
                    else {
                        b = 'maker';
                    }
                    
                    return `<span class="sts ${b}">${b}</span>`;
                };
                break;

            case 'side':
            case 'taker_type':
                c.render = function (data, type, full, meta) {
                    return data == 'buy' ? `<span class="sts upp">Buy</span>` : `<span class="sts dwn">Sell</span>`;
                };
                break;
    
            case 'actived': c.className = 'act'; c.data = 'state';
                c.render = function (data, type, full, meta) {
                    return `<span class="switch">Active<input id="swt-${++idx}" name="active" type="checkbox" value="${full.idkey}" ${data ? 'checked' : ''}/><label for="swt-${idx++}"></label></span>`;
                };
                break;
    
            case 'act_state': c.className = 'act'; c.data = 'state';
                c.render = function (data, type, full, meta) {
                    return `<span class="switch"><input id="swt-${++idx}" name="active" type="checkbox" value="${full.idkey}" ${data =='active' ? 'checked' : ''}/><label for="swt-${idx++}"></label></span>`;
                };
                break;
    
            case 'enable': c.className = 'act'; c.data = 'state';
                c.render = function (data, type, full, meta) {
                    return `<span class="switch">Enabled<input id="swt-${++idx}" name="enable" type="checkbox" value="${full.idkey}" ${data == 'enabled' ? 'checked' : ''}/><label for="swt-${idx++}"></label></span>`;
                };
                break;
    
            case 'resend': c.data='state';
                c.render = function (data, type, full, meta) {
                    if (data != 'pending') return '';
                    return `<button class="btn act" name="resend" value="${full.idkey}" data-method="POST" title="Gửi lại PIN"><i class="ico duot envelope"></i></button>`;
                };
                break;
                
            case 'verified': c.className = 'act';
                c.render = function (data, type, full, meta) {
                    return `<span class="switch">Verify<input id="swt-${idx++}" name="verify" type="checkbox" value="${full.idkey}" ${data ? 'checked' : ''}/><label for="swt-${idx++}"></label></span>`;
                };
                break;
    
            case 'locked': c.className = 'act';
                c.render = function (data, type, full, meta) {
                    if (data) {
                        return `<button class="btn dis" name="unlock" value="${full.idkey}"><i class="ico duot lock"></i></button>`;
                    } else {
                        var f = full['login_failed'];
                        return f ? `<small>Đăng nhập sai<br>${f} lượt</small>` : '';
                    }
                };
                break;
                
            case 'del': c.className = 'del';
                d = null;
                c.render = function (data, type, full, meta) {
                    return `<button class="btn dis" name="delete" value="${full.idkey}" title="Xóa bỏ"><i class="ico duot purge"></i></button>`;
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
            beforeSend: function(req) {
                req.setRequestHeader("Authorization", $.cookie('token') || '');
            },
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
            beforeSend: function(req) {
                req.setRequestHeader("Authorization", $.cookie('token') || '');
            },
            data: function (d) {
                return {
                    page: (parseInt(d.start / d.length) || 0) + 1,
                    size: d.length && d.length > 0 ? d.length : 15
                };
            },
            dataSrc: function (d) {
                return d.data || [];
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        };
    }

    var _clicker = _elem.data('clkpath');
    _elem.on('dblclick', 'tr', function (e) {
        e.preventDefault();
        if ($(this).parent('thead').length) return;

        var data = dtbl.row(this).data();
        if (!data) return;

        if (_clicker) {
            for (var v in _request) {
                var rx = new RegExp(`\\$\\{${v}\\}`, 'gi');
                _clicker = _clicker.replace(rx, _request[v]);
            }
            for (var v in data) {
                var rx = new RegExp(`\\$\\{${v}\\}`, 'gi');
                _clicker = _clicker.replace(rx, data[v]);
            }
            window.location.href = _clicker;
        }
        else if (_owner && _owner.frags) {
            _owner.frags.bind(0, data);
        }
    }).on('change', 'input[type=checkbox]', function (e) {
        e.preventDefault();
        var a=e.target;
        if (!a || a.name == 'sel') return;

        $.ajax({
            type: 'PATCH',
            url: $P(`${_path.replace('/all', '')}/${a.value}/${a.name}?state=${(!!a.checked).toString()}`),
            processData: false,
            headers: { "Authorization": $.cookie('token') || '' },
            success: function (json) {
                dtbl.ajax.reload();
                if (json.error) {
                    _notify.pushAlert(json.message);
                }
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        });
    }).on('click', 'button', function (e) {
        e.preventDefault();
        var a=$(this), _nme=a.attr('name'), _method=(a.data('method') || 'PATCH').toUpperCase(), _val=a.val();
        if (!a.length) return;

        if (_nme == 'delete') {
            $.ajax({
                type: 'DELETE',
                url: $P(`${_action.replace('/all', '')}/${_val}`),
                processData: false,
                headers: { "Authorization": $.cookie('token') || '' },
                success: function (json) {
                    dtbl.ajax.reload();
                    if (json.error) {
                        _notify.pushAlert(json.message);
                    }
                },
                error: function (xhr, sts, err) {
                    return raiseError(xhr);
                }
            });
        }
        else if (_method == 'POST') {
            $.ajax({
                type: _method || 'PATCH',
                url: $P(`${_path.replace('/all', '')}/${_val}/${_nme}`),
                processData: false,
                headers: { "Authorization": $.cookie('token') || '' },
                success: function (json) {
                    dtbl.ajax.reload();
                    if (json.error) {
                        _notify.pushAlert(json.message);
                    }
                },
                error: function (xhr, sts, err) {
                    return raiseError(xhr);
                }
            });
        }
        else {
            $.ajax({
                type: _method || 'PATCH',
                url: $P(`${_action.replace('/all', '')}/${_val}?${_nme}`),
                processData: false,
                headers: { "Authorization": $.cookie('token') || '' },
                success: function (json) {
                    dtbl.ajax.reload();
                    if (json.error) {
                        _notify.pushAlert(json.message);
                    }
                },
                error: function (xhr, sts, err) {
                    return raiseError(xhr);
                }
            });
        }
    }).on('click', 'a.caller', function (e) {
        var a=$(this), b = a.attr('href');
        if (!a.length || !b) return false;

        var mdl = ModuleView(b);
        if (!mdl) {
            e.preventDefault();
            e.stopPropagation();   
        }
        else {
            mdl.data('options', a.data('options'));
        }
    });

    var dtbl = _elem.DataTable(_config);
    dtbl.elem = _elem;
    dtbl.urlpath = _path;
    dtbl.refresh = function() {
        if (!_init) {
            _init = true;
            return;
        }
        _total = 0;
        _availed = 0;
        _locked = 0;
        _setted = 0;
        if (_path.indexOf("${")>=0) {
            dtbl.ajax.url($P(_path)).load();
        }
        else {
            dtbl.ajax.reload();
        }
    };

    dtbl.isElm = function(e) {
        return dtbl.elem && dtbl.elem.attr('id') == e.substring(1);
    };
    dtbl.selections = function() {
        var ids = [];
        $('input[type=checkbox]', dtbl.elem).each(function(){
            var a = this;
            if (a.checked && a.name == 'sel') ids.push(a.value);
        });
        return ids;
    };
    return dtbl;
}
function FragmentView(element, owner) {
    var frg = $(element);
    var _owner = owner;
    frg.show = function(i, b) {
        if (i) { frg.removeClass('hide'); }
        else if (!frg.hasClass('hide')) { frg.addClass('hide'); }
        if (b) _owner.refresh();
    };
    frg.isElm = function(e) {
        return frg && frg.attr('id') == e.substring(1);
    };
    
    frg.forms = [];
    $('form', frg).each(function(){
        var frm = FormerView(this, frg);
        frg.forms.push(frm);
    });
    frg.refresh = function() {
        for (var i = 0; i < frg.forms.length; i++) {
            frg.forms[i].refresh();
        }
    };
    frg.bind = function(d,i){
        frg.forms[i||0].bind(d);
    }
    return frg;
}
function FormerView(element, owner) {
    var frm = $(element);
    var _owner = owner;

    frm.isElm = function(e) {
        return frm && frm.attr('id') == e.substring(1);
    };
    frm.change = function(i) {
        if (i) {
            $('.altcap', this).html('Chỉnh sửa');
            frm.removeClass('append').addClass('change');
        }
        else {
            $('.altcap', this).html('Thêm mới');
            frm.removeClass('change').addClass('append');
        }
    };
    frm.refresh = function() {
        this.change(0);
        $('input', this).each(function(){
            var a = $(this), b = (a.attr('type') || '').toLowerCase(), c = a.attr('name');
            if (b == 'checkbox' || b == 'radio') {
                a.prop('checked', c == 'actived');
            }
            else {
                a.val('');
            }
        });
        $('select, textarea', this).each(function(){
            $(this).val('');
        });
    };
    frm.get = function(data, n) {
        if ('  ,last_changed,last_login,last_sent,locked_until,reg_date,created_at,'.indexOf(`,${n},`) > 0)
        {
            var d = new Date(data[n]);
            return `${$N(d.getHours())}:${$N(d.getMinutes())} ${$N(d.getDate())}/${$N(d.getMonth() + 1)}/${d.getFullYear()}`;
        }
        var c = '', req = _request;
        if (n.indexOf('.') > 0) {
            var nlst = n.split('.');
            c = data[nlst[0]];
            for (var i = 1; i < nlst.length; i++) {
                if (c) c = c[nlst[i]];
            }
        }
        else {
            c = data[n] || req[n];
        }

        if (c == undefined || c == null || `${c}` == '') {
            return '';
        }
        if (typeof(c) == 'object') {
            return c.code || c.idkey || c.id;
        }
        
        return $X(c);
    }
    frm.bind = function(data) {
        this.change(1);
        $('input', this).each(function() {
            var a = $(this), b = a.attr('name'), c = frm.get(data, b), t = (a.attr('type') || '').toLowerCase();
            if (b == '__keycode')
            {
                a.val(data.code || data.idkey || data.id);
            }
            else if (t == 'checkbox' || t == 'radio')
            {
                var arr = c.toString().split(',');
                a.prop('checked', arr.indexOf(a.val()) >= 0);
            }
            else if (!t) {
                var d = new Date(c);
                a.val(`${$N(d.getHours())}:${$N(d.getMinutes())} ${$N(d.getDate())}/${$N(d.getMonth() + 1)}/${d.getFullYear()}`);
            }
            else {
                a.val(c);
            }
        });
        $('select, textarea', this).each(function() {
            var a = $(this), b = a.attr('name'), c = b == 'role' ? frm.get(data, 'role.idkey') : frm.get(data, b);
            a.val(c);
        });
        $('label.fld', this).each(function() {
            var a = $(this), b = a.attr('name'), c = frm.get(data, b);
            a.html(c);
        });
        $('.hide-on, .show-on', this).each(function(){
            var a = $(this), b = a.data('field'), c = frm.get(data, b);
            if (c) {
                if (a.hasClass('hide-on')) { a.attr('class', `hide-on ${c}`); }
                else if (a.hasClass('show-on')) { a.attr('class', `show-on ${c}`); }
            }
        });
    };
    frm.submit(function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        var f = $(this), _data = {}, ins = f.hasClass('append'), upd = f.hasClass('change');
        var v = $('.identier', this);
        if (!v.length) { v = $('input[name="id"]', this); }
        if (upd && !v.length) {
            _notify.pushAlert('Can not find an Identity field!');
            return false;
        }
        var _id = upd ? v.val() : (ins ? 'new' : '');
        if (_id == '') {
            var b = $('button[type=submit]');
            if (b && b.val()) { _id = b.val(); }
        }
        if (_id) { _id = `/${_id}`; }

        $('input', f).each(function (i) {
            var a = $(this), b = a.attr('name'), c = a.val(), d = a.attr('id'), t = (a.attr('type') || '').toLowerCase();
            if (!b || d == 'idNo' || d == 'keyCode') return;
            if (c == '') { c = null; }
            
            if (t == 'hidden' && !c) {
                _data[b] = _request[b];
            }
            else if (t == 'checkbox' || t == 'radio') {
                if (c == 'true') {
                    _data[b] = a.prop('checked');
                }
                else if (a.prop('checked')) {
                    var s = (_data[b] || '');
                    _data[b] = `${s}${c != null && s != '' ? ',' : ''}${c}`;
                }
            }
            else {
                _data[b] = c;
            }
        });
        $('select, textarea', f).each(function (i) {
            var a = $(this), b = a.attr('name'), c = a.val(), d = a.attr('id');
            _data[b] = c == '' ? null : c;
        });

        var _refer = f.data('redirect'), _lgi = f.hasClass('signin'), _chgpsw = f.hasClass('repass');
        var _action = f.attr('action');
        $.ajax({
            type: 'POST',
            url: $P(`${_action}${_id}`),
            headers: { "Authorization": $.cookie('token') || '' },
            contentType: 'application/json; charset=UTF-8',
            data: JSON.stringify(_data),
            processData: false,
            success: function (json) {
                if (json.error)
                {
                    _notify.pushAlert(json.message);
                    return;
                }

                if (_lgi) {
                    $.cookie('name', json.data.name, { expires: 1 });
                    $.cookie('email', json.data.email, { expires: 1 });
                    $.cookie('token', `Bearer ${json.data.token}`, { expires: 1 });
                    window.location = _refer || 'index.html';
                    return;
                }

                if (_chgpsw) {
                    logout();
                    return;
                }

                if (_refer) {
                    //_self.refresh();
                    window.location = _refer;
                }
                else {
                    _owner.show(0,1);
                    //_self.refresh();
                }
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        });
    }).on('click', '.append .chg-only,.change .new-only', function(e){
        e.stopPropagation();
        return false;
    });
    
    $('[type=action]', frm).click(function(e){
        e.preventDefault();
        e.stopPropagation();
        var vi = $('.identier', frm);
        if (!vi.length) { vi = $('input[name="id"]', frm); }
        if (!vi.length) {
            _notify.pushAlert('Can not find an Identity field!');
            return false;
        }
        var _id = vi.val(), _val = $(this).val(), _pth = frm.attr('action');
        if (!_id || !_val || !_pth) return;

        $.ajax({
            type: 'PATCH',
            url: $P(`${_pth}/${_id}/${_val}`),
            headers: { "Authorization": $.cookie('token') || '' },
            contentType: 'application/json; charset=UTF-8',
            //data: JSON.stringify(_data),
            //processData: false,
            success: function (json) {
                if (json.error)
                {
                    _notify.pushAlert(json.message);
                    return;
                }
                
                _owner.show(0,1);
                //_self.refresh();
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        });
    });

    return frm;
}
function ModuleView(element) {
    var req = element.toString().indexOf('?');
    if (req > 0) {
        req = element.substring(req + 1);
        element = element.substring(0, element.length - req.length - 1);
    }
    
    _request = {};
    req = req.split ? req.split('&') : [];
    for (var i = 0; i < req.length; i++) {
        var r = req[i].split('=');
        if (r.length == 2) {
            _request[r[0]] = r[1];
        }
    }

    var _self = $(element.toLowerCase());
    if (_self && _self.length){
        var m = _module_lists[_self.attr('id')];
        if (m) return m;
    }
    else return 0;
    
    _self.caches = 0;
    _self.show = function (i) {
        if (!_self || !_self.length) return;
        if (i) { _self.removeClass('hide'); _self.reload(); }
        else if (!_self.hasClass('hide')) { _self.addClass('hide'); return; }
    };
    _self.isElm = function (e) {
        return _self && _self.attr('id') == e.substring(1);
    };

    if (_self && _self.attr('id')) { _module_lists[_self.attr('id')] = _self; }

    _self.balancer = $('#balancer', _self);
    _self.tables = [];
    $('.dtable', _self).each(function(){
        var dtbl = TableData(this, _self);
        _self.tables.push(dtbl);
    });

    _self.frags = [];
    $('.frag', _self).each(function(){
        var frg = FragmentView(this, _self);
        _self.frags.push(frg);
    });
    _self.frags.show = function(e,i){
        if (_self.frags.length == 1) {_self.frags[0].show(i);}
        else if (typeof e == 'number') {
            for (var j = 0; j < _self.frags.length; j++) {
                _self.frags[i].show(e == j ? i : 0);
            }
        }
        else {
            for (var j = 0; j < _self.frags.length; j++) {
                _self.frags[j].show(_self.frags[j].isElm(e) ? i : 0);
            }
        }
    };
    _self.frags.bind = function(e,d){
        if (_self.frags.length == 1) {
            _self.frags[0].show(1);
            _self.frags[0].bind(d);
        }
        else if (typeof e == 'number') {
            for (var j = 0; j < _self.frags.length; j++) {
                if (e == j) {
                    _self.frags[j].bind(d);
                    _self.frags[j].show(1);
                }
                else {
                    _self.frags[j].show(0);
                }
            }
        }
        else {
            for (var i = 0; i < _self.frags.length; i++) {
                if (_self.frags[j].isElm(e)) {
                    _self.frags[j].bind(d);
                    _self.frags[j].show(1);
                }
                else {
                    _self.frags[j].show(0);
                }
            }
        }
    };
    _self.frags.refresh = function(e){
        if (_self.frags.length == 1) {
            _self.frags[0].show(1);
            _self.frags[0].refresh();
        }
        else if (typeof e == 'number') {
            for (var j = 0; j < _self.frags.length; j++) {
                if (e == j) {
                    _self.frags[j].show(1);
                    _self.frags[j].refresh();
                }
                else {
                    _self.frags[j].show(0);
                }
            }
        }
        else {
            for (var j = 0; j < _self.frags.length; j++) {
                if (_self.frags[j].isElm(e)) {
                    _self.frags[j].show(1);
                    _self.frags[j].refresh();
                }
                else {
                    _self.frags[j].show(0);
                }
            }
        }
    };

    _self.forms = [];
    $('form.formee', _self).each(function(){
        var frm = FormerView(this, _self);
        _self.forms.push(frm);
    });

    $('.closer', _self).click(function(e){
        e.preventDefault();
        e.stopPropagation();
        _self.frags.show($(this).attr('href'), 0);
    });
    $('.adder', _self).click(function(e) {
        e.preventDefault();
        e.stopPropagation();
        _self.frags.refresh($(this).attr('href'));
    });
    $('.fresher', _self).click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        _self.refresh();
    });
    $('.picker', _self).each(function (i) {
        Picker(this);
    });

    _self.views = [];
    var _acc = $('.balancer', _self);
    if (_acc.length) {
        _self.views.push(_acc);
        _acc.refresh = function() {
            var tkr = _self.caches.ticker, prc = 0;
            if (tkr) {
                prc = parseFloat(tkr.price) || 0;
                $('.tkrcode', _self).html(tkr.code.toUpperCase());
                $('.tkrname', _self).html(tkr.name);
                $('.logo', _self).html(tkr.logo && tkr.logo != '#' ? `<img src="${data.ticker.logo}" />` : '');
            }

            var amt = parseFloat(_self.caches['balance']);
            if (tkr.code == 'vnd') $('.totalamt').html(`<h6>${$X(amt, 1, tkr.code)}</h6>`);
            else $('.totalamt').html(`<h6>${$X(amt, 1, tkr.code)}</h6><small>${$X(amt * prc, 1, 'VND')}</small>`);

            amt = parseFloat(_self.caches['avail_amount']);
            if (tkr.code == 'vnd') $('.availamt').html(`<h6>${$X(amt, 1, tkr.code)}</h6>`); 
            else $('.availamt').html(`<h6>${$X(amt, 1, tkr.code)}</h6><small>${$X(amt * prc, 1, 'VND')}</small>`);

            amt = parseFloat(_self.caches['lock_amount']);
            if (tkr.code == 'vnd') $('.lockamt').html(`<h6>${$X(amt, 1, tkr.code)}</h6>`);
            else $('.lockamt').html(`<h6>${$X(amt, 1, tkr.code)}</h6><small>${$X(amt * prc, 1, 'VND')}</small>`);
        };
    }
    var _depFrm = $('#formDep');
    if (_depFrm.length) {
        _self.views.push(_depFrm);
        _depFrm.refresh=function(){
            var addr = _self.caches['deposit_address'];
            $('#depAddress', _depFrm).val(addr||'');
            if (addr) {
                _depFrm.removeClass('append').addClass('change');
            }
            else {
                _depFrm.removeClass('change').addClass('append');
            }
        }
        _depFrm.submit(function(e){
            e.preventDefault();
            e.stopPropagation();
            
            var _action = _depFrm.attr('action');
            $.ajax({
                type: 'POST',
                url: $P(_action),
                headers: { "Authorization": $.cookie('token') || '' },
                contentType: 'application/json; charset=UTF-8',
                success: function (json) {
                    if (json.error)
                    {
                        _notify.pushAlert(json.message);
                        return;
                    }
                    
                    _self.caches = json.data;
                    _depFrm.refresh();
                },
                error: function (xhr, sts, err) {
                    return raiseError(xhr);
                }
            });
        });
    }

    _self.refresh = function() {
        if (_self.tables.length) _self.frags.show(0);
        $.each(_self.tables, function(){
            this.refresh();
        });
        $.each(_self.views, function(){
            this.refresh(_self.caches);
        });
        $('a[data-href]', _self).each(function(){
            var a=$(this), h = a.data('href');
            a.attr('href', $P(h));
        });
    };
    _self.reload = function() {
        var pth = _self.data('action');
        if (!pth) {
            _self.refresh();
            return;
        }
        
        $.ajax({
            type: 'GET',
            url: $P(pth),
            headers: { "Authorization": $.cookie('token') || '' },
            success: function (json) {
                if (json.error)
                {
                    _notify.pushAlert(json.message);
                    return;
                }
                
                _self.caches = json.data;
                _self.refresh();
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        });
    };

    return _self;
}
function Ordering(element, owner) {
    var frm = $(element);
    var _owner = owner;

    frm.isElm = function(e) {
        return frm && frm.attr('id') == e.substring(1);
    };
    frm.caches = {
        isBuy: frm.isElm('#buying'),
        isLim: 1,
        amt: 0,
        vol: 0,
        prc: 0,
    };

    frm.prc = $('input.price', frm)
    frm.vol = $('input.volume', frm);
    frm.tol = $('input.total', frm);
    frm.prcMsk = $('.price-mask', frm);
    frm.prc.on('input', function(){
        var v = parseFloat(frm.vol.val()) || 0, p = parseFloat(frm.prc.val()) || 0, t = p * v;
        var r = frm.caches.isBuy ? frm.caches.amt ? parseInt(t * 100 / frm.caches.amt) : 0 :  frm.caches.vol ? parseInt(v * 100 / frm.caches.vol) : 0;
        if (p) {
            if (frm.isBuy) {
                if (t > frm.caches.amt) {
                    t = frm.caches.amt;
                    v = t / p;
                }
            }
            else {
                if (v > frm.caches.vol) {
                    v = frm.caches.vol;
                    t = v * p;
                }
            }
            frm.vol.val($X(v));
            frm.tol.val($X(t));
            frm.ran.inp.val(r < 0 ? 0 : (r > 100 ? 100 : r));
            frm.setRange(r);
        }
        else {
            frm.vol.val('');
            frm.tol.val('');
            frm.ran.inp.val(0);
            frm.setRange(0);
        }
    });
    frm.vol.on('input',function(){
        var v = parseFloat(frm.vol.val()) || 0, p = frm.caches.isLim ? (parseFloat(frm.prc.val()) || 0) : frm.caches.prc, t = !p || !v ? 0 : p * v;
        var r = frm.caches.isBuy ? frm.caches.amt ? parseInt(t * 100 / frm.caches.amt) : 0 :  frm.caches.vol ? parseInt(v * 100 / frm.caches.vol) : 0;
        if (v) {
            if (frm.isBuy && t > frm.caches.amt) {
                t = frm.caches.amt;
                p = t / v;
            }
            
            if (frm.caches.isLim) {
                frm.prc.val($X(p));
                frm.tol.val($X(t));
            }
            else {
                frm.tol.val($X(t));
                frm.tol.prop('disabled', true);
            }
            frm.ran.inp.val(r < 0 ? 0 : (r > 100 ? 100 : r));
            frm.setRange(r);
        }
        else {
            frm.vol.val('');
            frm.tol.val('');
            frm.tol.prop('disabled', false);
            frm.ran.inp.val(0);
            frm.setRange(0);
        }
    });
    frm.tol.on('input', function(){
        var t = parseFloat(frm.tol.val()) || 0, p = frm.caches.isLim ? (parseFloat(frm.prc.val()) || 0) : frm.caches.prc, v = !p || !t ? 0 : t / p;
        var r = frm.caches.isBuy ? frm.caches.amt ? parseInt(t * 100 / frm.caches.amt) : 0 :  frm.caches.vol ? parseInt(v * 100 / frm.caches.vol) : 0;
        if (t) {
            if (frm.isBuy) {
                if (t > frm.caches.amt) {
                    t = frm.caches.amt;
                    p = t / v;
                }
            }
            else {
                if (v > frm.caches.vol) {
                    v = frm.caches.vol;
                    p = t / v;
                }
            }
            if (frm.caches.isLim) {
                frm.prc.val($X(p));
                frm.vol.val($X(v));
            }
            else {
                frm.vol.val($X(v));
                frm.vol.prop('disabled', true);
            }
            frm.ran.inp.val(r < 0 ? 0 : (r > 100 ? 100 : r));
            frm.setRange(r);
        }
        else {
            frm.tol.val('');
            frm.vol.val('');
            frm.vol.prop('disabled', false);
            frm.ran.inp.val(0);
            frm.setRange(0);
        }
    });

    frm.swchs = $('.swch-lst input', frm);
    frm.swchs.change(function(){
        frm.swchs.refresh();
    });
    frm.swchs.refresh=function() {
        frm.swchs.each(function() {
            var a = $(this);
            if (a.prop('checked')) {
                frm.caches.isLim = (a.val() == 'limit');
                return 0;
            }
        });

        frm.prc.removeClass('hide');
        frm.prcMsk.removeClass('hide');
        if (frm.caches.isLim) frm.prcMsk.addClass('hide');
        else frm.prc.addClass('hide');
        
        frm.ran.refresh(0);
        frm.prc.val('');
        frm.vol.val('');
        frm.tol.val('');
    };

    frm.ran = $('.ranger', frm);
    frm.ran.inp = $('[type=range]', frm.ran);
    frm.ran.lbl = $('label', frm.ran);
    frm.ran.inp.on('input change', function() {
        frm.ran.refresh(parseInt(frm.ran.inp.val()));
    });
    frm.setRange=function(i) {
        if (typeof(i) == 'number') {
            frm.ran.lbl.removeClass('dwn');
            if (i > 100) {
                frm.ran.lbl.addClass('dwn').html('>100' + frm.ran.data('unit'));
            }
            else {
                frm.ran.lbl.html(i.toString() + frm.ran.data('unit') || '');
            }
            i = i < 0 ? 0 : (i > 100 ? 100 : i);
        }
    };
    frm.ran.refresh=function(i) {
        frm.setRange(i);
        var t = frm.caches.amt, r = parseInt(frm.ran.inp.val()), v = frm.caches.vol;
        if (frm.caches.isBuy) {
            t = (!t || !r ? 0 : t * r) / 100;

            if (t == 0) {
                frm.vol.val('');
                frm.tol.val('');
                if (!frm.caches.isLim) {
                    frm.vol.prop('disabled', false);
                    frm.tol.prop('disabled', false);
                }
            }
            else {
                var p = frm.caches.isLim ? parseFloat(frm.prc.val()) : frm.caches.prc, v = !p ? 0 : t / p;
                frm.tol.val($X(t));
                frm.vol.val($X(v));
                if (!frm.caches.isLim && !frm.tol.prop('disabled')) frm.vol.prop('disabled', true);
            }
        }
        else {
            v = (!v || !v ? 0 : v * r) / 100;

            if (v == 0) {
                frm.vol.val('');
                frm.tol.val('');
                if (!frm.caches.isLim) {
                    frm.vol.prop('disabled', false);
                    frm.tol.prop('disabled', false);
                }
            }
            else {
                var p = frm.caches.isLim ? parseFloat(frm.prc.val()) : frm.caches.prc, t = !p ? 0 : v * p;
                frm.tol.val($X(t));
                frm.vol.val($X(v));
                if (!frm.caches.isLim && !frm.vol.prop('disabled')) frm.tol.prop('disabled', true);
            }
        }
    };

    frm.bind = function(data) {
        frm.caches.prc = frm.caches.isBuy ? data['max_price'] :  data['min_price'];
        frm.caches.amt = frm.caches.isBuy ? data['quote_account'].balance : data['base_account'].balance * frm.caches.prc;
        frm.caches.vol = frm.caches.isBuy ? data['quote_account'].balance / frm.caches.prc : data['base_account'].balance;
        
        $('[data-ref]', frm).each(function() {
            var a = $(this), b = a.data('ref');
            if (b == 'base_unit') a.html(data['base_account'].ticker.toUpperCase());
            else if (b == 'quote_unit') a.html(data['quote_account'].ticker.toUpperCase());
            else if (b == 'total_amt') {
                var html = '';
                if (frm.caches.isBuy) {
                    html = `Đang có: <b>${$X(frm.caches.amt, 1)}</b> <small>${data['quote_account'].ticker.toUpperCase()}</small><br>`;
                    html += `Mua được: <b>${$X(frm.caches.vol, 1)}</b> <small>${data['base_account'].ticker.toUpperCase()}</small>`;
                }
                else
                {
                    html = `Đang có: <b>${$X(frm.caches.vol, 1)}</b> <small>${data['base_account'].ticker.toUpperCase()}</small><br>`;
                    html += `Bán được: <b>${$X(frm.caches.amt, 1)}</b> <small>${data['quote_account'].ticker.toUpperCase()}</small>`;
                }
                a.html(html);
            }
            else a.html($X(data[b], 1));
        });
    };
    frm.refresh = function() {
        frm.swchs.prop('checked', false);
        frm.swchs.eq(0).prop('checked', true);
        frm.swchs.refresh();
        
        frm.vol.prop('disabled', false);
        frm.tol.prop('disabled', false);
        frm.ran.inp.prop('disabled', false);
    };
    
    frm.submit(function (e) {
        e.preventDefault();
        e.stopPropagation();
        
        var _data = {}, upd = frm.hasClass('change');
        var v = $('.identier', this);
        if (!v.length) { v = $('input[name="id"]', this); }
        if (upd && !v.length) {
            _notify.pushAlert('Can not find an Identity field!');
            return false;
        }
        var _id = '/new';
        if (upd) _id = `/${v.val()}`;
        else {
            var b = $('button[type=submit]');
            if (b && b.val()) { _id = `/${b.val()}`; }
        }

        $('input', frm).each(function (i) {
            var a = $(this), b = a.attr('name'), c = a.val(), d = a.attr('id'), t = (a.attr('type') || '').toLowerCase();
            if (!b || d == 'idNo' || d == 'keyCode') return;
            if (c == '') { c = null; }
            console.log([a,b,c]);
            
            if (t == 'hidden' && !c) {
                _data[b] = _request[b] || _owner.caches[b];
            }
            else if (t == 'checkbox' || t == 'radio') {
                if (c == 'true') {
                    _data[b] = a.prop('checked');
                }
                else if (a.prop('checked')) {
                    var s = (_data[b] || '');
                    _data[b] = `${s}${c != null && s != '' ? ',' : ''}${c}`;
                }
            }
            else {
                _data[b] = c;
            }
        });
        $('select, textarea', frm).each(function (i) {
            var a = $(this), b = a.attr('name'), c = a.val(), d = a.attr('id');
            _data[b] = c == '' ? null : c;
        });
        
        var _action = `${frm.attr('action')}${_id}`;
        if (_owner.path && typeof(_owner.path) == 'function') _action = _owner.path(_action);
        $.ajax({
            type: 'POST',
            url: $P(_action),
            headers: { "Authorization": $.cookie('token') || '' },
            contentType: 'application/json; charset=UTF-8',
            data: JSON.stringify(_data),
            processData: false,
            success: function (json) {
                if (json.error)
                {
                    _notify.pushAlert(json.message);
                    return;
                }
                
                _owner.refresh();
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        });
    });

    return frm;
}
function XchangeView(element) {
    var _self = $(element);
    if (_self && _self.length){
        var m = _module_lists[_self.attr('id')];
        if (m) return m;
    }
    else return 0;
    
    var m = location.toString();
    var req = m.indexOf('?');
    if (req > 0) {
        req = m.substring(req + 1);
        m = m.substring(0, m.length - req.length - 1);
    }
    
    _request = {};
    req = req.split ? req.split('&') : [];
    for (var i = 0; i < req.length; i++) {
        var r = req[i].split('=');
        if (r.length == 2) {
            _request[r[0]] = r[1];
        }
    }
    
    _self.caches = {};
    _self.path = function(url) {
        url = url.toString();
        var rx = new RegExp(`\\$\\{instr\\}`, 'gi');
        url = url.replace(rx, _request['instr']);
        if (url.indexOf('${') >= 0) {
            for (var v in _request) {
                var rx = new RegExp(`\\$\\{${v}\\}`, 'gi');
                url = url.replace(rx, _request[v]);
            }
            for (var v in _self.caches) {
                var rx = new RegExp(`\\$\\{${v}\\}`, 'gi');
                url = url.replace(rx, _request[v]);
            }
        }
        return url;
    }

    _self.market = $('#instruments', _self);
    var _market_changing = 0;
    _self.change = function() {
        if (_market_changing) {
            clearTimeout(_market_changing);
            _market_changing = 0;
        }
        _market_changing = setTimeout(function(){
            _self.refresh();
        }, 10);
    };
    _self.getInstr = function(){
        return _self.market.val();
    };
    _self.market.change(function() {
        window.location = `index.html?instr=${_self.market.val()}`;
        _self.change();
    });
    _self.mktinf = $('#marketinfo');
    _self.mktinf.refresh = function() {
        var _action = _self.path(_self.mktinf.data('action')), _method = _self.mktinf.data('method');
        $.ajax({
            type: _method,
            url: $P(_action),
            headers: { "Authorization": $.cookie('token') || '' },
            contentType: 'application/json; charset=UTF-8',
            processData: false,
            success: function (json) {
                if (json.error) {
                    _notify.pushAlert(json.message);
                    return;
                }
                if (json.data) {
                    _self.caches = json.data;
                    $('#marketinfo [data-ref], #curstate [data-ref]').each(function(){
                        var a = $(this), b = a.data('ref'), c = b == 'change_percent' ? $X(_self.caches[b], 1) : $X(_self.caches[b], 1, '%');
                        a.html(c || _request[b]);
                    });
                }
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        });
    };
    
    _self.orders = [];
    $('.ordering').each(function(){
        _self.orders.push(Ordering(this, _self));
    });
    _self.orders.refresh = function(e, d) {
        for (var i = 0; i < _self.orders.length; i++) {
            if (_self.orders[i].isElm(e)) {
                _self.orders[i].removeClass('hide');
                _self.orders[i].refresh();
            }
            else if (!_self.orders[i].hasClass('hide')) {
                _self.orders[i].addClass('hide');
            }
        };
    }
    _self.buysell = $('#buseTabs a');
    _self.buysell.click(function(e){
        e.preventDefault();
        var a = $(this);
        if (a.hasClass('active')) return;
        
        _self.buysell.removeClass('active');
        a.addClass('active');
        _self.orders.refresh(a.attr('href'));
    });
    _self.buysell.refresh = function() {
        var ac = $('#buseTabs'), _action = _self.path(ac.data('action')), _method = ac.data('method');
        $.ajax({
            type: _method,
            url: $P(_action),
            headers: { "Authorization": $.cookie('token') || '' },
            contentType: 'application/json; charset=UTF-8',
            processData: false,
            success: function (json) {
                if (json.error) {
                    _notify.pushAlert(json.message);
                    return;
                }
                if (json.data) {
                    for (var i = 0; i < _self.orders.length; i++) {
                        _self.orders[i].bind(json.data);
                    }
                }
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        });
        _self.buysell.each(function(){
            if ($(this).hasClass('active')) _self.orders.refresh($(this).attr('href'));
        });
    }
    _self.tables = [];
    _self.tables.refresh = function(){
        for (var i =0; i< _self.tables.length; i++) {
            _self.tables[i].refresh();
        }
    };

    _self.cancel = $('#cancelOrder');
    _self.cancel.click(function(e){
        e.preventDefault();
        var _tbl = 0, _lst = [];
        for (var i =0; i< _self.tables.length; i++) {
            if (_self.tables[i].isElm('#dtbl2')) {
                _tbl = _self.tables[i];
                _lst = _tbl.selections();
            }
        }

        if (!_lst.length) {
            alert('Vui lòng đánh dấu chọn ít nhất 1 lệnh bên dưới!')
            return;
        }

        $.ajax({
            type: 'POST',
            url: $P(`${_self.cancel.attr('href')}`),
            headers: { "Authorization": $.cookie('token') || '' },
            contentType: 'application/json; charset=UTF-8',
            data: JSON.stringify(_lst),
            processData: false,
            success: function (json) {
                if (json.error)
                {
                    _notify.pushAlert(json.message);
                    return;
                }

                _tbl.refresh();
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        });
    });

    _self.init = function() {
        var _action = _self.market.data('action');
        if (!_action) return;
        
        $.ajax({
            type: 'GET',
            url: $P(_action),
            headers: { "Authorization": $.cookie('token') || '' },
            contentType: 'application/json; charset=UTF-8',
            processData: false,
            success: function (json) {
                if (json.error) {
                    _notify.pushAlert(json.message);
                    return;
                }
                if (json.data && json.data.length) {
                    var d = json.data;
                    _self.market.empty();
                    for (var i = 0; i < d.length; i++) {
                        if (typeof(d[i]) == 'object') $(`<option value="${d[i].key}">${d[i].key.toUpperCase()}</option>`).appendTo(_self.market);
                        else $(`<option value="${d[i]}">${d[i]}</option>`).appendTo(_self.market);
                    }

                    if (!_request['instr']) {
                        _request['instr'] = d[0].key || d[0];
                    }
                    _self.market.val(_request['instr']);
                    _self.change();

                    $('.dtable', _self).each(function() {
                        var dtbl = TableData(this, _self);
                        _self.tables.push(dtbl);
                    });
                }
            },
            error: function (xhr, sts, err) {
                return raiseError(xhr);
            }
        });
        return _self;
    };
    _self.refresh = function() {
        _self.mktinf.refresh();
        _self.buysell.refresh();
        _self.tables.refresh();
    };

    return _self.init();
}

var _navigate = {}, _notify = {}, _module = {};

$(document).ready(function(){
    _body = $('body');
    var _isAdmin = _body.hasClass('admin');
    var _isAuth = ($.cookie('token') || '').indexOf('Bearer ') > -1;
    if (_isAuth) {
        _body.addClass('authed');
    }
    else {
        if (_body.hasClass('must-auth')) {
            window.location = 'auth.html';
        }
    }

    _navigate = $('.sidenav') || {};
    _navigate.change=function(e){
        if (!_navigate.length) return;
        e = e.indexOf('#') == 0 ? e : `#${e}`;
        $('a', _navigate).each(function() {
            var a = $(this);
            if (a.attr('href') == e) {
                if (!a.hasClass('active')) { a.addClass('active'); }
            }
            else {
                a.removeClass('active');
            }
            if (!_isAdmin || $(window).width() < 1201) { 
                var b = $('body');
                if (b.hasClass('has-side')) b.removeClass('has-side');
            }
        });
    };
    if (_navigate.length) {
        $('.menuer').click(function(e){
            e.preventDefault();
            $('body').toggleClass('has-side');
        });

        if (_isAdmin && $(window).width() > 1200) { $('.menuer').click(); }
    }
    $('.darker').click(function(e){
        e.preventDefault();
        $('body').toggleClass('darke');
    });
    $('.logout').click(function(e){
        logout();
    });

    var _noti_show = 0;
    _notify = $('.sidebar.noti-list') || {};
    _notify.counter = $('.notifier small');
    _notify.tipter = $('.msgtip');
    _notify.show=function(e){
        if (!e) {
            _notify.removeClass('active');
            _notify.setCounter(0);
        }
        else if (!_notify.hasClass('active')) _notify.addClass('active');
    };
    _notify.setCounter = function(e){
        _notify.counter.html(e ? e.toString() : '');
    };
    _notify.addCounter = function(e){
        var a = parseInt(_notify.counter.html() || 0) + e;
        _notify.counter.html(e ? e.toString() : '');
    };
    _notify.pushMsg = function(msg, tmr, typ){
        if (_noti_show) {
            clearTimeout(_noti_show);
            _noti_show = 0;
        }
        _noti_show = setTimeout(function(){
            $('.msg-cont', _notify.tipter).html(msg);
            if (_notify.tipter.hasClass('hide')) {
                _notify.tipter.attr('class', `msgtip${typ ? ' ' + typ : ''}`);

                if (_noti_show) {
                    clearTimeout(_noti_show);
                    _noti_show = 0;
                }
                _noti_show = setTimeout(function(){
                    if (!_notify.tipter.hasClass('hide')) _notify.tipter.addClass('hide');
                }, tmr || 5000);
            }
        }, 10);
    };
    _notify.pushAlert = function(msg){
        _notify.pushMsg(msg, 3000, 'alert');
    };
    _notify.pushError = function(msg){
        _notify.pushMsg(msg, 5000, 'error');
    };
    _notify.pushOk = function(msg){
        _notify.pushMsg(msg, 3000, 'succ');
    };
    $('.notifier').click(function(e){
        e.preventDefault();
        var a = $(this).toggleClass('active');
        _notify.show(a.hasClass('active'));
    });
    _notify.tipter.click(function(e){
        e.preventDefault();
        if (_noti_show) {
            clearTimeout(_noti_show);
            _noti_show = 0;
        }
        _notify.tipter.attr('class', `msgtip hide`);
    });

    if (_body.hasClass('xchange')) {
        XchangeView(_body);
    }
    else {
        _module = 0;
        var _module_default = '';
        $('section').each(function(){
            var s = $(this), id = s.attr('id');
            if (id && !s.hasClass('hide')) {
                s.addClass('hide');
                _module_default = `#${id}`;
            }
            return false;
        });

        var _module_changing = 0;
        function moduleChange() {
            var m = (location.hash || _module_default);
            var mdl = ModuleView(m);
            if (_module == mdl) return;
            if (_module_changing) {
                clearTimeout(_module_changing);
                _module_changing = 0;
            }
            
            _module_changing = setTimeout(function() {
                if(_module) { _module.show(0); }
                _module = mdl;
                _module.show(1);
                _navigate.change(m);
            }, 100);
        }
        $(window).on('hashchange', function() {
            moduleChange();
        });

        window.refresh = function(){
            moduleChange();
        };
        moduleChange();
    }
});