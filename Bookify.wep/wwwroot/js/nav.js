// Hide navbar on scroll down, show on scroll up
(function(){
  var doc = document;
  var lastScroll = window.pageYOffset || doc.documentElement.scrollTop;
  var navbar = doc.querySelector('.navbar');
  if(!navbar) return;
  var ticking = false;
  var threshold = 10; // minimal scroll to act

  function onScroll(){
    var current = window.pageYOffset || doc.documentElement.scrollTop;
    // ignore small deltas
    if(Math.abs(current - lastScroll) <= threshold){
      ticking = false;
      return;
    }

    if(current > lastScroll && current > 100){
      // scrolling down -> hide
      navbar.classList.add('nav-hidden');
    } else {
      // scrolling up -> show
      navbar.classList.remove('nav-hidden');
    }

    lastScroll = current <= 0 ? 0 : current;
    ticking = false;
  }

  window.addEventListener('scroll', function(){
    if(!ticking){
      window.requestAnimationFrame(onScroll);
      ticking = true;
    }
  }, {passive:true});
})();
