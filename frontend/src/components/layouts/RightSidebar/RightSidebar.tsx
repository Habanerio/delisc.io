export const RightSidebar = ({ children }: { children: React.ReactNode }) => {
   return (
      <div className='hidden lg:block lg:w-1/4 lg:pl-4'>
         <div className='sticky top-0'>
            <div className='bg-white shadow-sm rounded-lg p-4 mb-4'>{children}</div>
         </div>
      </div>
   );
};
