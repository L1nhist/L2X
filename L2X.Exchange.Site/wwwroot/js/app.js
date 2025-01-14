"use strict";
(function (factory) {
	if (typeof define === 'function' && define.amd) define(['jquery'], factory);
    else if (typeof exports === 'object') factory(require('jquery'));
    else factory(jQuery);
}(function ($) {
    function Menubar(e) {
        let _elem = $(e);
        _elem.current = 0;
        _elem.items = [];
        $('.menu-item').each(function(i){
            let a = $(this), n = a.attr('href');
            if (n) _elem.items.push(a);
        });

        _elem.change = function(e) {
            e = e.indexOf('#') == 0 ? e.substring(1) : e;
            for (let i = 0; i < _elem.items.length; i++) {
                let n = _elem.items[i].attr('href').substring(1);
                if (e == n) {
                    if (!_elem.items[i].hasClass('active')) {
                        _elem.items[i].addClass('active');
                    }
                }
                else {
                    _elem.items[i].removeClass('active');
                }
            }
        }

        return _elem;
    }
    function Dtablet(e) {
        let _elem = $(e).Datable();

        _elem.refresh = function() {
            _elem.ajax.reload();
            if(_elem.refreshing) _elem.refreshing();
        };

        return _elem;
    }
    function Panelet(e) {
        let _elem = $(e);
        _elem.panels = [];
        $('> .pane-view', _elem).each(function(i) {
            let pnl = Panelet(this);
            _elem.panels.push(pnl);
        });

        _elem.refresh = function() {
            for (let i = 0; i < _elem.panels.length; i++) {
                _elem.panels[i].refresh();
            }
            if(_elem.refreshing) _elem.refreshing();
        };

        return _elem;
    }
    function Tabpanes(e) {
        let _elem = $(e);

        _elem.tabs = $('.tab-item', _elem);
        _elem.current = _elem.tabs.index('.active');
        _elem.default = _elem.tabs.index('.tab-main') || 0;
        _elem.panels = [];
        $('> .pane-view', _elem).each(function(i) {
            let pnl = Panelet(this);
            _elem.panels.push(pnl);
        });

        _elem.change = function(i) {
            i = parseInt(i);
            if (i == _elem.current) return;

            for (let j = 0; j < _elem.tabs.length; j++) {
                if (i == j) {
                    if (_elem.tabs.eq(j).hasClass('active'))
                    {
                        _elem.tabs.eq(j).addClass('active');
                        if (_elem.panels.length > j) {
                            _elem.panels[j].show(1);
                            _elem.panels[j].refresh();
                        }
                    }
                }
                else {
                    _elem.tabs.eq(j).removeClass('active');
                    _elem.panels[j].show(0);
                }
            }
        };
        _elem.refresh = function() {
            _elem.change(_elem.default);
            if(_elem.refreshing) _elem.refreshing();
        };

        return _elem;
    }
    function Framelet(e) {
        let _elem = $(e);

        _elem.show = function() {
            if (s == undefined) _elem.show(!_elem.hasClass('hide'));
            else if (s) { _elem.removeClass('hide'); _elem.refresh(); }
            else if (!_elem.hasClass('hide')) _elem.addClass('hide');
        };
        _elem.refresh = function() {
            for (let i = 0; i < _elem.tabpanes.length; i++) {
                _elem.tabpanes[i].refresh();
            }
            if(_elem.refreshing) _elem.refreshing();
        };

        _elem.panels = [];
        $('> .pane-view', _elem).each(function(i) {
            let pnl = Panelet(this);
            _elem.panels.push(pnl);
        });
        _elem.tabpanes = [];
        $('.tab-view', _elem).each(function(i) {
            let tab = Tabpanes(this);
            _elem.tabs.push(tab);
        });

        return _elem;
    }
    function Module(e) {
        let _elem = $(e);
        _elem.mdlName = _elem.attr('id');
        _elem.tables = [];
        $('table.dtable', _elem).each(function(i) {
            let tbl = Dtablet(this);
            _elem.tables.push(tbl);
        });

        _elem.frames = [];
        $('.frag', _elem).each(function(i) {
            let frm = Framelet(this);
            _elem.frames.push(frm);
        });

        _elem.show=function(s) {
            if (s == undefined) _elem.show(!_elem.hasClass('hide'));
            else if (s) { _elem.removeClass('hide'); _elem.refresh(); }
            else if (!_elem.hasClass('hide')) _elem.addClass('hide');
        };
        _elem.refresh=function() {
            for (let i = 0; i < _elem.tables.length; i++) {
                _elem.tables[i].refresh();
            }
            for (let i = 0; i < _elem.frames.length; i++) {
                _elem.frames[i].show(0);
            }
            if(_elem.refreshing) _elem.refreshing();
        };

        return _elem;
    }
    function AppLoader() {
        let _app = {};
        _app.body = $('body').eq(0);
        _app.default = _app.body.data('default') || '';
        _app.authed = _app.body.hasClass('must-auth');
        _app.current = '';
        _app.menu = Menubar('.menubar');
        _app.modules = {};
        $('section.modulet').each(function(i) {
            let mdl = Module(this);
            if (!mdl.mdlName) mdl.mdlName = `module${i}`;
            _app.modules[mdl.mdlName] = mdl;
            if (!mdl.hasClass('hide') && !_app.default) _app.default = mdl.mdlName;
        });
        _app.modules.change = function(e) {
            let e = e.indexOf('#') == 0 ? e.substring(1) : e;
            for (let i = 0; i < _app.modules.length; i++) {
                if (_app.modules[i].mdlName == e) {
                    if (_app.modules[i].hasClass('hide')) {
                        _app.modules[i].removeClass('hide');
                        _app.modules[i].refresh();
                    }
                }
                else if (!_app.modules[i].hasClass('hide')) _app.modules[i].addClass('hide');
            }
        };
        _app.refresh = function() {
            var e = window.location.hash;
            e = (e || '#').substring(1) || _app.default;
            if (_app.current != e)
            {
                _app.current = e;
                _app.menu.change(e);
                _app.modules.change(e);
            }
        };

        return _app;
    }

    let app = {};
    $(document).ready(function() {
        app = AppLoader();
    });
    $(window).on('hashchange', function() {
        app.refresh();
    });
    $(window).refresh(function(){
        app.refresh();
    });
}));