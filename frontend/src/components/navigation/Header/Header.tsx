import Link from 'next/link';
import { NavigationMenu } from '@/components/ui/navigation-menu';

export const Header = () => {
   return (
      <header>
         <nav className='fixed w-full z-50 mt-2'>
            <div className='container mx-auto px-4'>
               <div className='flex items-center justify-between p-4 h-16 border-1 border-black rounded-lg'>
                  <Link href='/' className='text-xl font-bold'>
                     Deliscio
                  </Link>

                  <NavigationMenu className=' max-w mx-auto mt-4'>
                     <Link href='/' className='text-2xl font-bold'>
                        <span className='text-emerald-500'>Home</span>
                     </Link>
                     <Link href='/tags' className='text-2xl font-bold'>
                        <span className='text-emerald-500'>Tags</span>
                     </Link>
                     <Link href='/profile' className='text-2xl font-bold'>
                        <span className='text-emerald-500'>Profile</span>
                     </Link>
                  </NavigationMenu>
               </div>
            </div>
         </nav>
      </header>
   );
};
