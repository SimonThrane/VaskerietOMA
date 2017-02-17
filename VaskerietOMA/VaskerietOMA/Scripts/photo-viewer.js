
var request;
var $current;
var cache[];
var $frame = $('#photo-viewer');
var $thumbs=$('.thumbs');

function crossfade($img){
    if($current){
        $current.stop().fadeOut('slow');
    }

    $img.css({
        marginLeft: -$img.width()/2,
        marginLeft: -$img.height()/2

    });

    $img.stop().fadeOut('slow', 1);

    $current=$img;

}

    $(document).on('click', 'thumb', function(e){
        var $img;
        var scr=this.href;
        request=scr;

        e.preventDefault();
        
        $thumbs.removeClass('active');
        $(this).addClass('active');

        if(cache.hasOwnProperty(src)){
            if(cache[src].isLoading===false){
                crossfade(cache[src].$img);
            }
        }else{
            $img=$('<img></img>');
            cache[src]={
                $img: $img, 
                isLoading: true;
        };

        $img.on('load', function(){
            $img.hide();
            $frame.removeClass('is-loading').append($img);

            cache[src].isLoading=false;

            if(request===src){
                crossfade($img);
            }
        });

        $frame.addClass('is-loading');

        $img.attr({
            'src':src,
        'alt': this.title || ''
    });
        }

    });
    $('.thumb').eq(0).click();